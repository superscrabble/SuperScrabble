using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using SuperScrabble.Data;
using SuperScrabble.Data.Common.Repositories;
using SuperScrabble.Data.Models;
using SuperScrabble.Data.Repositories;
using SuperScrabble.Data.Seeding;
using SuperScrabble.Services.Common;
using SuperScrabble.Services.Data.Games;
using SuperScrabble.Services.Data.Users;
using SuperScrabble.Services.Data.Words;
using SuperScrabble.Services.Game;
using SuperScrabble.Services.Game.Common.GameplayConstantsProviders;
using SuperScrabble.Services.Game.Common.TilesProviders;
using SuperScrabble.Services.Game.Factories;
using SuperScrabble.Services.Game.Matchmaking;
using SuperScrabble.Services.Game.Scoring;
using SuperScrabble.Services.Game.Validation;
using SuperScrabble.WebApi.Hubs;
using SuperScrabble.WebApi.Timers;

var builder = WebApplication.CreateBuilder(args);

string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection") ?? DatabaseConfig.ConnectionString;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies().UseSqlServer(connectionString));

// Fails fast when the signing key is missing or too short.
string jwtSigningKey = builder.Configuration["Jwt:SigningKey"]!;
var encryptionKeyProvider = new ConfigurationEncryptionKeyProvider(jwtSigningKey);

AddCors(builder.Services);

AddIdentityOptions(builder.Services);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

AddJsonWebTokenBearerAuthentication(builder.Services, encryptionKeyProvider.GetEncryptionKey());

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddHostedService<GameBackgroundService>();

// Data
builder.Services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

// Services.Common
builder.Services.AddTransient<IShuffleService, ShuffleService>();
builder.Services.AddTransient<IJsonWebTokenGenerator, JsonWebTokenGenerator>();
builder.Services.AddSingleton<IEncryptionKeyProvider>(encryptionKeyProvider);
builder.Services.AddTransient<IInvitationCodeGenerator, InvitationCodeGenerator>();

// Services.Data
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IWordsService, WordsService>();

// Services.Game
builder.Services.AddSingleton<TimerManager>();
builder.Services.AddScoped<IGameValidator, GameValidator>();
builder.Services.AddScoped<IGameplayConstantsProvider, GameplayConstantsProvider>();
builder.Services.AddScoped<ITilesProvider, StandardTilesProvider>();
builder.Services.AddScoped<IMatchmakingService, MatchmakingService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameStateFactory, GameStateFactory>();
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<IGamesService, GamesService>();

builder.Services.AddSpaStaticFiles(options =>
{
    options.RootPath = "ClientApp/dist";
});

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    AppDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    // The word files live in the WebApi project's "all" folder; resolve them relative to the
    // content root so seeding works regardless of the process working directory.
    string wordsDirectory = builder.Configuration["Seeding:WordsDirectory"] is { Length: > 0 } configured
        ? configured
        : Path.Combine(app.Environment.ContentRootPath, "all");

    new AppSeeder(wordsDirectory)
        .SeedAsync(dbContext, serviceScope.ServiceProvider)
        .GetAwaiter()
        .GetResult();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
}

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<GameHub>("/gamehub");
});

app.UseSpa(options =>
{
    options.Options.SourcePath = "ClientApp";
});

app.Run();

static void AddIdentityOptions(IServiceCollection services)
{
    services
        .AddIdentity<AppUser, AppRole>(options =>
        {
            options.SignIn.RequireConfirmedEmail = false;
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters =
                "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789_";

            options.Password = new PasswordOptions
            {
                RequireDigit = true,
                RequiredLength = 8,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = true,
            };
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
}

// Backend publish in the same folder put /dist (angular)

static void AddCors(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder
                .WithOrigins("https://localhost:4200", "http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
}

static void AddJsonWebTokenBearerAuthentication(IServiceCollection services, string encryptionKey)
{
    var keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken)
                    && path.StartsWithSegments("/gamehub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },
            OnTokenValidated = async context =>
            {
                var userManager = context.HttpContext
                    .RequestServices.GetRequiredService<UserManager<AppUser>>();

                // The token carries only a name claim (no user id), so the user must be
                // looked up by name — and from the token's principal, not HttpContext.User,
                // which is not populated yet at this point in the pipeline.
                string? userName = context.Principal?.Identity?.Name;

                AppUser? user = userName is null
                    ? null
                    : await userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    context.Fail("Unauthorized");
                }
            }
        };

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
}

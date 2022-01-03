using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SuperScrabble.Data;
using SuperScrabble.Data.Common.Repositories;
using SuperScrabble.Data.Models;
using SuperScrabble.Data.Repositories;
using SuperScrabble.Services.Common;
using SuperScrabble.Services.Data.Users;
using SuperScrabble.Services.Data.Words;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();

/*builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins("https://localhost:4200", "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});*/

builder.Services
    .AddIdentity<AppUser, AppRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.User.RequireUniqueEmail = true;

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

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

AddJsonWebTokenBearerAuthentication(builder.Services);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Data
builder.Services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

// Services.Common
builder.Services.AddTransient<IShuffleService, ShuffleService>();
builder.Services.AddTransient<IJsonWebTokenGenerator, JsonWebTokenGenerator>();
builder.Services.AddTransient<IEncryptionKeyProvider, InMemoryEncryptionKeyProvider>();

// Services.Data
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IWordsService, WordsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void AddJsonWebTokenBearerAuthentication(IServiceCollection services)
{
    string encryptionKey = new InMemoryEncryptionKeyProvider().GetEncryptionKey();
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
            OnTokenValidated = context =>
            {
                var userManager = context.HttpContext
                    .RequestServices.GetRequiredService<UserManager<AppUser>>();

                var user = userManager.GetUserAsync(context.HttpContext.User);

                if (user == null)
                {
                    context.Fail("Unauthorized");
                }

                return Task.CompletedTask;
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

namespace SuperScrabble.WebApi
{
    using System.Text;
    using System.Globalization;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using Microsoft.OpenApi.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    using SuperScrabble.Data;
    using SuperScrabble.Common;
    using SuperScrabble.Models;
    using SuperScrabble.WebApi.Hubs;
    using SuperScrabble.Services.Data;

    using static SuperScrabble.Common.ModelValidationConstraints;
    using SuperScrabble.Services.Game;
    using SuperScrabble.Services;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    //Front-end cors
                    builder
                        .WithOrigins("https://localhost:4200", "http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services
                .AddIdentity<AppUser, AppRole>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = false;
                    options.User.RequireUniqueEmail = true;

                    options.Password = new PasswordOptions
                    {
                        RequireDigit = Password.RequireDigit,
                        RequiredLength = Password.MinLength,
                        RequireLowercase = Password.RequireLowercase,
                        RequireUppercase = Password.RequireUppercase,
                        RequireNonAlphanumeric = Password.RequireNonAlphanumeric,
                        RequiredUniqueChars = Password.RequiredUniqueChars,
                    };
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            AddLocalization(services);

            AddJwtBearerAuthentication(services);

            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<IShuffleService, ShuffleService>();
            services.AddTransient<ITilesProvider, MyOldBoardTilesProvider>();
            services.AddTransient<IBonusCellsProvider, MyOldBoardBonusCellsProvider>();
            services.AddTransient<IGameStateManager, StaticGameStateManager>();

            services.AddControllers();
            services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SuperScrabble.WebApi", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SuperScrabble.WebApi v1"));
            }

            app.UseHttpsRedirection();

            UseLocalization(app);

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/gamehub");
            });
        }

        private static void UseLocalization(IApplicationBuilder app)
        {
            var localizeOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizeOptions.Value);
        }

        private static void AddLocalization(IServiceCollection services)
        {
            const string English = "en-US";
            const string Bulgarian = "bg-BG";

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo(English),
                    new CultureInfo(Bulgarian),
                };

                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.DefaultRequestCulture = new RequestCulture(culture: English, uiCulture: English);
            });
        }

        private static void AddJwtBearerAuthentication(IServiceCollection services)
        {
            var keyBytes = Encoding.UTF8.GetBytes(GlobalConstants.EncryptionKey);

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

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/gamehub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
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
    }
}

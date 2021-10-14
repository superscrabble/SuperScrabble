namespace SuperScrabble.WebApi
{
    using Microsoft.OpenApi.Models;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;

    using Microsoft.Extensions.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), 
                        b => b.MigrationsAssembly("SuperScrabble.WebApi")));

            services
                .AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedEmail = false)
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiScopes(Config.GetAPIScopes())
                .AddInMemoryApiResources(Config.GetAPIs())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<AppUser>();

            services
                .AddAuthentication(Config.Bearer)
                .AddJwtBearer(Config.Bearer, options =>
                {
                    options.Authority = "https://localhost:44307";
                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.Audience = Config.WebApi;
                });

            services.AddControllers();

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

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

namespace SuperScrabble.Data
{
    using SuperScrabble.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
    using IdentityServer4.EntityFramework.Options;
    using Microsoft.Extensions.Options;

    public class AppDbContext : ApiAuthorizationDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options,
               IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-6KN27TH\\SQLEXPRESS;Database=SuperScrabble;Trusted_Connection=True;");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>();
        }
    }
}

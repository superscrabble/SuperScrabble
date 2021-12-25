namespace SuperScrabble.Data
{
    using SuperScrabble.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore.Proxies;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Word> Words { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<UserGame> UsersGames { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies().UseSqlServer(DatabaseConfig.ConnectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .Entity<UserGame>()
                .HasKey(userGame => new 
                { 
                    userGame.GameId,
                    userGame.UserId
                });

            builder
                .Entity<UserGame>()
                .HasOne(userGame => userGame.Game)
                .WithMany(game => game.Users)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<UserGame>()
                .HasOne(userGame => userGame.User)
                .WithMany(user => user.Games)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Word>()
                .HasIndex(word => word.Value);

            RenameDefaultIdentityModels(builder);
        }

        private static void RenameDefaultIdentityModels(ModelBuilder builder)
        {
            builder.Entity<AppUser>(entity => entity.ToTable("Users"));

            builder.Entity<AppRole>(entity => entity.ToTable("Roles"));

            builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UserRoles"));

            builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims"));

            builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins"));

            builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("RoleClaims"));

            builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens"));
        }
    }
}

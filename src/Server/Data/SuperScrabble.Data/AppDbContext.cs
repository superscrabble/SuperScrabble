using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

using SuperScrabble.Data.Models;

namespace SuperScrabble.Data;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Word> Words { get; set; } = default!;

    public DbSet<Game> Games { get; set; } = default!;

    public DbSet<GameUser> GamesUsers { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(DatabaseConfig.ConnectionString);
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder
            .Entity<GameUser>()
            .HasIndex(gameUser => new
            {
                gameUser.GameId,
                gameUser.UserId,
            })
            .IsUnique();

        builder
            .Entity<GameUser>()
            .HasOne(gameUser => gameUser.Game)
            .WithMany(game => game.Users)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Entity<GameUser>()
            .HasOne(gameUser => gameUser.User)
            .WithMany(user => user.Games)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Entity<Word>()
            .HasIndex(word => word.Value)
            .IsUnique();

        RenameDefaultIdentityTablesNames(builder);
    }

    private static void RenameDefaultIdentityTablesNames(ModelBuilder builder)
    {
        builder.Entity<AppUser>(entity => entity.ToTable("Users"));

        builder.Entity<AppRole>(entity => entity.ToTable("Roles"));

        builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UsersRoles"));

        builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UsersClaims"));

        builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UsersLogins"));

        builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("RolesClaims"));

        builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UsersTokens"));
    }
}

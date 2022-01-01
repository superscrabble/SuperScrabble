namespace SuperScrabble.Data.Seeding
{
    public interface ISeeder
    {
        Task SeedAsync(AppDbContext dbContext, IServiceProvider serviceProvider);
    }
}

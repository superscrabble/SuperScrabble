namespace SuperScrabble.Data.Seeding
{
    public class AppSeeder : ISeeder
    {
        public async Task SeedAsync(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            ISeeder[] seeders =
            {
                new WordsSeeder()
            };

            foreach (ISeeder seeder in seeders)
            {
                await seeder.SeedAsync(dbContext, serviceProvider);
            }
        }
    }
}

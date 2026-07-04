namespace SuperScrabble.Data.Seeding
{
    public class AppSeeder : ISeeder
    {
        private readonly string? wordsDirectory;

        public AppSeeder(string? wordsDirectory = null)
        {
            this.wordsDirectory = wordsDirectory;
        }

        public async Task SeedAsync(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            ISeeder[] seeders =
            {
                new WordsSeeder(this.wordsDirectory)
            };

            foreach (ISeeder seeder in seeders)
            {
                await seeder.SeedAsync(dbContext, serviceProvider);
            }
        }
    }
}

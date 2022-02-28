namespace SuperScrabble.Data.Seeding
{
    using SuperScrabble.Data.Models;

    public class WordsSeeder : ISeeder
    {
        public async Task SeedAsync(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            string[] files = Directory.GetFiles("../../../../resources/final-list/all", "*.txt");

            foreach (string file in files)
            {
                string[] uniqueWords = await File.ReadAllLinesAsync(file);
                await AddWordsToDbContextAsync(uniqueWords.Distinct(), dbContext);
            }
        }

        private static async Task AddWordsToDbContextAsync(IEnumerable<string> words, AppDbContext dbContext)
        {
            foreach (string word in words)
            {
                await dbContext.Words.AddAsync(new Word { Value = word });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}

namespace SuperScrabble.Data.Seeding
{
    using SuperScrabble.Data.Models;

    public class WordsSeeder : ISeeder
    {
        public async Task SeedAsync(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            string[] files = Directory.GetFiles("./all", "*.txt");

            var allWords = new List<string>();

            foreach (string file in files)
            {
                string[] uniqueWords = await File.ReadAllLinesAsync(file);
                allWords.AddRange(uniqueWords);
            }

            await AddWordsToDbContextAsync(allWords.Select(w => w.ToLower()).Distinct(), dbContext);
        }

        private static async Task AddWordsToDbContextAsync(IEnumerable<string> words, AppDbContext dbContext)
        {
            foreach (string word in words)
            {
                if (dbContext.Words.Any(w => w.Value == word))
                {
                    continue;
                }

                await dbContext.Words.AddAsync(new Word { Value = word });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}

namespace SuperScrabble.Data.Seeding
{
    using SuperScrabble.Data.Models;

    public class WordsSeeder : ISeeder
    {
        public async Task SeedAsync(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            string[] files = Directory.GetFiles("../../../resources/final-list/all", "*.txt");

            foreach (string file in files)
            {
                string[] lines = await File.ReadAllLinesAsync(file);
                await AddWordsToDbContextAsync(lines, dbContext);
            }
        }

        private static async Task AddWordsToDbContextAsync(string[] words, AppDbContext dbContext)
        {
            foreach (string word in words)
            {
                string trimmedWord = word.Trim();

                bool doesWordExist = dbContext.Words.FirstOrDefault(w => w.Value == trimmedWord) != null;

                if (doesWordExist)
                {
                    continue;
                }

                await dbContext.Words.AddAsync(new Word { Value = trimmedWord });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}

namespace SuperScrabble.Data.Seeding
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using SuperScrabble.Models;

    public class WordsSeeder : ISeeder
    {
        public async Task SeedAsync(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            string[] arr = Directory.GetFiles("../../../resources/final-list/all", "*.txt");
            string[] lines;

            foreach (string file in arr)
            {
                lines = await File.ReadAllLinesAsync(file);
                await AddWordsToDbContextAsync(lines, dbContext);
            }
        }

        private async Task AddWordsToDbContextAsync(string[] words, AppDbContext dbContext)
        {
            foreach (string word in words)
            {
                string trimmedWord = word.Trim();
                Console.WriteLine(trimmedWord);
                await dbContext.Words.AddAsync(new Word() { Value = trimmedWord });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}

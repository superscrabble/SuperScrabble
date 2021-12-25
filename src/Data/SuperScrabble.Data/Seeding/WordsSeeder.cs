namespace SuperScrabble.Data.Seeding
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using SuperScrabble.Models;

    public class WordsSeeder : ISeeder
    {
        public async Task SeedAsync(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            string[] arr = Directory.GetFiles("../../../resources/final-list/all", "*.txt");
            string[] lines;

            Console.OutputEncoding = System.Text.Encoding.UTF8;

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
                
                var doesWordExist = dbContext.Words.FirstOrDefault(x => x.Value == trimmedWord) != null;
                
                if(doesWordExist)
                {
                    continue;
                }
                await dbContext.Words.AddAsync(new Word() { Value = trimmedWord });
            }

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

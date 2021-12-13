using SuperScrabble.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.Data.Seeding
{
    public class WordsSeeder : ISeeder
    {
        public async Task SeedAsync(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            //TODO: remove the non-scrabble words
            Console.OutputEncoding = Encoding.UTF8;
            
            string[] arr = Directory.GetFiles("../../../resources/words", "*.txt");
            string[] lines;

            foreach (string file in arr)
            {
                lines = await File.ReadAllLinesAsync(file);
                Console.WriteLine(file);
                await AddWordsToDbContextAsync(lines, dbContext);
                Console.WriteLine(file);
            }

            Console.WriteLine("Count: ");
            Console.WriteLine(dbContext.Words.Count());

            //Console.WriteLine(lines)
        }

        private async Task AddWordsToDbContextAsync(string[] words, AppDbContext dbContext)
        {
            foreach (string word in words)
            {
                string trimmedWord = word.Trim();

                bool isAlreadyInDb = dbContext.Words.Any(x => x.Value == trimmedWord);
                
                if (!isAlreadyInDb)
                {
                    await dbContext.Words.AddAsync(new Word() { Value = trimmedWord });
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}

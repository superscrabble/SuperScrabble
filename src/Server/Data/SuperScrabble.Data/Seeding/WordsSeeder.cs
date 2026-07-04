namespace SuperScrabble.Data.Seeding
{
    using System.Diagnostics;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using SuperScrabble.Data.Models;

    public class WordsSeeder : ISeeder
    {
        private const int InsertBatchSize = 10_000;

        private readonly string? wordsDirectory;

        public WordsSeeder(string? wordsDirectory = null)
        {
            this.wordsDirectory = wordsDirectory;
        }

        public async Task SeedAsync(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            var logger = (ILogger?)serviceProvider.GetService(typeof(ILogger<WordsSeeder>));

            if (await dbContext.Words.AnyAsync())
            {
                logger?.LogInformation("Words table is not empty. Skipping word seeding.");
                return;
            }

            string directory = this.wordsDirectory ?? "./all";

            if (!Directory.Exists(directory))
            {
                logger?.LogWarning(
                    "Word list directory '{Directory}' not found " +
                    "(override with the 'Seeding:WordsDirectory' configuration value). Word seeding skipped.",
                    directory);
                return;
            }

            var stopwatch = Stopwatch.StartNew();

            // Case-insensitive dedup: the unique index on Word.Value uses the database's
            // case-insensitive collation, so "Ева" and "ева" would collide on insert.
            var uniqueWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string file in Directory.GetFiles(directory, "*.txt"))
            {
                foreach (string line in await File.ReadAllLinesAsync(file))
                {
                    string word = line.Trim();

                    if (word.Length > 0)
                    {
                        uniqueWords.Add(word.ToLowerInvariant());
                    }
                }
            }

            logger?.LogInformation(
                "Seeding {WordCount} unique words from '{Directory}'...",
                uniqueWords.Count, Path.GetFullPath(directory));

            bool autoDetectChanges = dbContext.ChangeTracker.AutoDetectChangesEnabled;
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            try
            {
                var batch = new List<Word>(InsertBatchSize);

                foreach (string word in uniqueWords)
                {
                    batch.Add(new Word { Value = word });

                    if (batch.Count == InsertBatchSize)
                    {
                        await SaveBatchAsync(dbContext, batch);
                    }
                }

                await SaveBatchAsync(dbContext, batch);
            }
            finally
            {
                dbContext.ChangeTracker.AutoDetectChangesEnabled = autoDetectChanges;
            }

            logger?.LogInformation(
                "Seeded {WordCount} words in {ElapsedSeconds:F1}s.",
                uniqueWords.Count, stopwatch.Elapsed.TotalSeconds);
        }

        private static async Task SaveBatchAsync(AppDbContext dbContext, List<Word> batch)
        {
            if (batch.Count == 0)
            {
                return;
            }

            dbContext.Words.AddRange(batch);
            await dbContext.SaveChangesAsync();

            // Detach saved entities so the change tracker does not grow across batches.
            dbContext.ChangeTracker.Clear();
            batch.Clear();
        }
    }
}

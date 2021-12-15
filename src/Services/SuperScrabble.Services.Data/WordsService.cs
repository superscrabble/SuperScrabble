namespace SuperScrabble.Services.Data
{
    using System.Linq;
    using System.Collections.Generic;

    using SuperScrabble.Models;
    using SuperScrabble.Data.Repositories;

    public class WordsService : IWordsService
    {
        private readonly IRepository<Word> wordsRepository;

        public WordsService(IRepository<Word> wordsRepository)
        {
            this.wordsRepository = wordsRepository;
        }

        public bool AreAllWordsValid(IEnumerable<string> words)
        {
            foreach (string word in words)
            {
                if (!this.IsWordValid(word))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsWordValid(string word)
        {
            return this.wordsRepository.All().FirstOrDefault(w => w.Value.ToLower() == word) != null;
        }
    }
}

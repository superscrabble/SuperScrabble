namespace SuperScrabble.Services.Game
{
    using System.Collections.Generic;

    using SuperScrabble.Services.Data;

    public class AlwaysValidWordsService : IWordsService
    {
        public bool AreAllWordsValid(IEnumerable<string> words)
        {
            return true;
        }

        public bool IsWordValid(string word)
        {
            return true;
        }
    }
}

namespace SuperScrabble.Services.Game
{
    using SuperScrabble.Services.Data;
    using System.Collections.Generic;

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

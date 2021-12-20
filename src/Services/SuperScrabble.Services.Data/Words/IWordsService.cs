namespace SuperScrabble.Services.Data.Words
{
    using System.Collections.Generic;

    public interface IWordsService
    {
        bool IsWordValid(string word);

        bool AreAllWordsValid(IEnumerable<string> words);
    }
}

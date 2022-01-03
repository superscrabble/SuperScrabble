namespace SuperScrabble.Services.Data.Words
{
    public interface IWordsService
    {
        bool IsWordValid(string word);

        bool AreAllWordsValid(IEnumerable<string> words);
    }
}

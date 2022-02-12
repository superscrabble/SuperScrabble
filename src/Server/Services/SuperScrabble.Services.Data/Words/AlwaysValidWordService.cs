namespace SuperScrabble.Services.Data.Words
{
    public class AlwaysValidWordsService : IWordsService
    {
        public bool IsWordValid(string? word)
        {
            return true;
        }
    }
}

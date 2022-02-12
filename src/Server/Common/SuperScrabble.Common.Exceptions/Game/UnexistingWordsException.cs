namespace SuperScrabble.Common.Exceptions.Game
{
    public class UnexistingWordsException : ValidationFailedAfterInputTilesHaveBeenPlacedException
    {
        public UnexistingWordsException(IEnumerable<string> unexistingWords)
        {
            this.UnexistingWords = unexistingWords;
        }

        public IEnumerable<string> UnexistingWords { get; }
    }
}

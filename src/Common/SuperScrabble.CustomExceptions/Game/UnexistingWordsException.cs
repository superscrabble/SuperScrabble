namespace SuperScrabble.CustomExceptions.Game
{
    using System.Collections.Generic;

    using SuperScrabble.LanguageResources;

    public class UnexistingWordsException : ValidationFailedAfterInputTilesHaveBeenPlacedException
    {
        public UnexistingWordsException(IEnumerable<string> unexistingWords)
            : base(nameof(Resource.WordDoesNotExist), Resource.WordDoesNotExist)
        {
            this.UnexistingWords = new List<string>(unexistingWords);
        }

        public IEnumerable<string> UnexistingWords { get; }
    }
}

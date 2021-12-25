using SuperScrabble.LanguageResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.CustomExceptions.Game
{
    public class UnexistingWordsException : ValidationFailedException
    {
        public UnexistingWordsException(IEnumerable<string> unexistingWords) : base(nameof(Resource.WordDoesNotExist), Resource.WordDoesNotExist)
        {
            this.UnexistingWords = new List<string>(unexistingWords);
        }

        public IEnumerable<string> UnexistingWords { get; }
    }
}

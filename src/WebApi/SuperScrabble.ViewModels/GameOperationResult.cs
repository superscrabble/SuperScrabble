namespace SuperScrabble.ViewModels
{
    using System.Collections.Generic;

    public class GameOperationResult
    {
        public GameOperationResult()
        {
            this.ErrorsByCodes = new Dictionary<string, string>();
            this.UnexistingWords = new List<string>();
        }

        public bool IsSucceeded { get; set; }

        public IDictionary<string, string> ErrorsByCodes { get; }

        public List<string> UnexistingWords { get; }
    }
}

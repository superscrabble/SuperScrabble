namespace SuperScrabble.WebApi.ViewModels.Games
{
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

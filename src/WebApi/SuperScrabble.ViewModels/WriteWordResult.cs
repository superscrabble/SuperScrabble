namespace SuperScrabble.ViewModels
{
    using System.Collections.Generic;

    public class WriteWordResult
    {
        public WriteWordResult()
        {
            this.ErrorsByCodes = new Dictionary<string, string>();
        }

        public bool IsSucceeded { get; set; }

        public IDictionary<string, string> ErrorsByCodes { get; set; }
    }
}

namespace SuperScrabble.ViewModels
{
    using System.Collections.Generic;

    public class ModelStateErrorViewModel
    {
        public string PropertyName { get; init; }

        public string DisplayName { get; init; }

        public IEnumerable<string> ErrorMessages { get; init; }
    }
}

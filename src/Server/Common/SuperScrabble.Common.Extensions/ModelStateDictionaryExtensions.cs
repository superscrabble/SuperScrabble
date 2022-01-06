namespace SuperScrabble.Common
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public static class ModelStateDictionaryExtensions
    {
        public static IEnumerable<string> AllErrorMessages(this ModelStateDictionary modelState)
        {
            return modelState.SelectMany(
                entry => entry.Value.Errors.Select(err => err.ErrorMessage));
        }
    }
}

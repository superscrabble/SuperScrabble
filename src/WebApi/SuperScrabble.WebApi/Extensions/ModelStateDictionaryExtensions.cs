namespace SuperScrabble.WebApi.Extensions
{
    using SuperScrabble.ViewModels;

    using System.Linq;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public static class ModelStateDictionaryExtensions
    {
        public static IEnumerable<ModelStateErrorViewModel> Errors(this ModelStateDictionary modelState)
        {
            return modelState.Keys.Select(key => new ModelStateErrorViewModel
            {
                PropertyName = key,
                ErrorMessages = modelState[key].Errors.Select(error => error.ErrorMessage)
            });
        }
    }
}

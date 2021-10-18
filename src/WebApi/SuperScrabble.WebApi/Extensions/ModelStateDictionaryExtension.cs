namespace SuperScrabble.WebApi.Extensions
{
    using SuperScrabble.Utilities;
    using SuperScrabble.ViewModels;

    using System.Linq;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public static class ModelStateDictionaryExtension
    {
        public static IEnumerable<ModelStateErrorViewModel> GetErrors<TInputModel>(this ModelStateDictionary modelState)
        {
            return modelState.Keys.Select(key => new ModelStateErrorViewModel
            {
                PropertyName = key,
                DisplayName = AttributesGetter.DisplayName<TInputModel>(key),
                ErrorMessages = modelState[key].Errors.Select(err => err.ErrorMessage)
            });
        }
    }
}

namespace SuperScrabble.InputModels
{
    using SuperScrabble.LanguageResources;
    using SuperScrabble.CustomValidationAttributes.ResourceAttributes;

    using System.ComponentModel.DataAnnotations;

    public class LoginInputModel
    {
        [Display(Name = nameof(Resource.UserNameDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.UserNameIsRequired))]
        public string UserName { get; init; }

        [Display(Name = nameof(Resource.PasswordDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.PasswordIsRequired))]
        public string Password { get; init; }
    }
}

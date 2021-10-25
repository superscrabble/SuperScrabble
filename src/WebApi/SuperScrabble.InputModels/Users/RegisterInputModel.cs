namespace SuperScrabble.InputModels.Users
{
    using SuperScrabble.LanguageResources;
    using SuperScrabble.CustomValidationAttributes.Email;
    using SuperScrabble.CustomValidationAttributes.Password;
    using SuperScrabble.CustomValidationAttributes.SymbolRequiring;
    using SuperScrabble.CustomValidationAttributes.ResourceAttributes;

    using System.ComponentModel.DataAnnotations;

    public class RegisterInputModel
    {
        [Display(Name = nameof(Resource.UserNameDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.UserNameIsRequired))]

        public string UserName { get; init; }


        [Display(Name = nameof(Resource.EmailAddressDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.EmailAddressIsRequired))]
        [ValidEmailAddress]

        public string Email { get; init; }

        [Display(Name = nameof(Resource.PasswordDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.PasswordIsRequired))]

        [RequireDigit]
        [RequireLowercase]
        [RequireUppercase]
        [PasswordMinLength]
        [RequireNonAlphanumeric]

        public string Password { get; init; }

        [Display(Name = nameof(Resource.RepeatedPasswordDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.RepeatedPasswordIsRequired))]

        public string RepeatedPassword { get; init; }
    }
}

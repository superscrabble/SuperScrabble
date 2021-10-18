namespace SuperScrabble.InputModels.Users
{
    using SuperScrabble.LanguageResources;
    using SuperScrabble.CustomValidationAttributes.Password;
    using SuperScrabble.CustomValidationAttributes.SymbolRequiring;
    using SuperScrabble.CustomValidationAttributes.ResourceAttributes;

    using System.ComponentModel.DataAnnotations;

    public class UpdatePasswordInputModel
    {
        [Display(Name = nameof(Resource.OldPasswordDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.OldPasswordIsRequired))]
        public string OldPassword { get; init; }

        [Display(Name = nameof(Resource.NewPasswordDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.NewPasswordIsRequired))]

        [RequireDigit]
        [RequireLowercase]
        [RequireUppercase]
        [PasswordMinLength]
        [RequireNonAlphanumeric]
        public string NewPassword { get; init; }
    }
}

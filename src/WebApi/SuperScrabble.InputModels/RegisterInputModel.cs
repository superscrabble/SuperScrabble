namespace SuperScrabble.InputModels
{
    using SuperScrabble.CustomValidationAttributes;
    using SuperScrabble.LanguageResources;

    using System.ComponentModel.DataAnnotations;

    using static SuperScrabble.LanguageResources.ResourceNames;
    using static SuperScrabble.Common.ModelValidationConstraints.Password;

    public class RegisterInputModel
    {
        // UserName

        [Display(Name = UserNameDisplayName, ResourceType = typeof(Resource))]

        [Required(
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = UserNameIsRequired)]

        public string UserName { get; init; }

        // Email Address

        [Display(Name = "EmailAddressDisplayName", ResourceType = typeof(Resource))]

        [Required(
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "EmailAddressIsRequired")]

        [EmailAddress(
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "EmailAddressIsInvalid")]

        public string Email { get; init; }

        // Password

        [Display(Name = PasswordDisplayName, ResourceType = typeof(Resource))]

        [Required(
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = PasswordIsRequired)]

        [PasswordMinLength(
            MinLength,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "PasswordIsTooShort")]

        [RequireDigit(
            RequireDigit,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "PasswordRequiresDigit")]

        [RequireLowercase(
            RequireLowercase,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "PasswordRequiresLowercase")]

        [RequireUppercase(
            RequireUppercase,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "PasswordRequiresUppercase")]

        [RequireNonAlphanumeric(
            RequireNonAlphanumeric,
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "PasswordRequiresNonAlphanumeric")]

        public string Password { get; init; }

        // Repeated Password

        [Display(Name = "RepeatedPasswordDisplayName", ResourceType = typeof(Resource))]

        [Required(
            ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "RepeatedPasswordIsRequired")]

        public string RepeatedPassword { get; init; }
    }
}

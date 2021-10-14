namespace SuperScrabble.InputModels
{
    using SuperScrabble.LanguageResources;
    using System.ComponentModel.DataAnnotations;

    using static SuperScrabble.LanguageResources.ResourceNames;

    public class RegisterInputModel
    {
        [Display(Name = UserNameDisplayName, ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = UserNameIsRequired)]
        public string UserName { get; init; }

        [Display(Name = "EmailAddressDisplayName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "EmailAddressIsRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "EmailAddressIsInvalid")]
        public string Email { get; init; }

        [Display(Name = PasswordDisplayName, ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = PasswordIsRequired)]
        public string Password { get; init; }

        [Display(Name = "RepeatedPasswordDisplayName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RepeatedPasswordIsRequired")]
        public string RepeatedPassword { get; init; }
    }
}

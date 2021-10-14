namespace SuperScrabble.InputModels
{
    using SuperScrabble.LanguageResources;
    using System.ComponentModel.DataAnnotations;

    using static SuperScrabble.LanguageResources.ResourceNames;

    public class LoginInputModel
    {
        [Display(Name = UserNameDisplayName, ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = UserNameIsRequired)]
        public string UserName { get; init; }

        [Display(Name = PasswordDisplayName, ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = PasswordIsRequired)]
        public string Password { get; init; }
    }
}

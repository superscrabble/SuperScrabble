namespace SuperScrabble.InputModels.Users
{
    using SuperScrabble.LanguageResources;
    using SuperScrabble.CustomValidationAttributes.ResourceAttributes;

    using System.ComponentModel.DataAnnotations;

    public class UpdateUserNameInputModel
    {
        [Display(Name = nameof(Resource.NewUserNameDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.NewUserNameIsRequired))]
        public string NewUserName { get; init; }
    }
}

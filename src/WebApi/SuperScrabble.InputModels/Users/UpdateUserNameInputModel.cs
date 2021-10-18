namespace SuperScrabble.InputModels.Users
{
    using SuperScrabble.CustomValidationAttributes.ResourceAttributes;
    using SuperScrabble.LanguageResources;

    using System.ComponentModel.DataAnnotations;

    public class UpdateUserNameInputModel
    {
        [Display(Name = nameof(Resource.OldUserNameDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.OldUserNameIsRequired))]
        public string OldUserName { get; init; }

        [Display(Name = nameof(Resource.NewUserNameDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.NewUserNameIsRequired))]
        public string NewUserName { get; init; }
    }
}

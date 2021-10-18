namespace SuperScrabble.InputModels.Users
{
    using SuperScrabble.CustomValidationAttributes.ResourceAttributes;
    using SuperScrabble.LanguageResources;

    using System.ComponentModel.DataAnnotations;

    public class UpdatePasswordInputModel
    {
        [Display(Name = nameof(Resource.OldPasswordDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.OldPasswordIsRequired))]
        public string OldPassword { get; init; }

        [Display(Name = nameof(Resource.OldPasswordDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.NewPasswordIsRequired))]
        public string NewPassword { get; init; }
    }
}

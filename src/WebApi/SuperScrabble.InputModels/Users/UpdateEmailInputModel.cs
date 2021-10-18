namespace SuperScrabble.InputModels.Users
{
    using SuperScrabble.LanguageResources;
    using SuperScrabble.CustomValidationAttributes.Email;
    using SuperScrabble.CustomValidationAttributes.ResourceAttributes;

    using System.ComponentModel.DataAnnotations;

    public class UpdateEmailInputModel
    {
        [Display(Name = nameof(Resource.OldEmailAddressDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.EmailAddressIsRequired))]
        [ValidEmailAddress]
        public string OldEmail { get; init; }

        [Display(Name = nameof(Resource.NewEmailAddressDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.EmailAddressIsRequired))]
        [ValidEmailAddress]
        public string NewEmail { get; init; }
    }
}

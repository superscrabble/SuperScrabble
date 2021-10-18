namespace SuperScrabble.InputModels.Users
{
    using SuperScrabble.CustomValidationAttributes.ResourceAttributes;
    using SuperScrabble.LanguageResources;
    using System.ComponentModel.DataAnnotations;

    public class UpdateEmailInputModel
    {
        [Display(Name = nameof(Resource.OldEmailAddressDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.EmailAddressIsInvalid))]
        public string OldEmail { get; init; }

        [Display(Name = nameof(Resource.NewEmailAddressDisplayName), ResourceType = typeof(Resource))]
        [ResourceRequired(nameof(Resource.EmailAddressIsInvalid))]
        public string NewEmail { get; init; }
    }
}

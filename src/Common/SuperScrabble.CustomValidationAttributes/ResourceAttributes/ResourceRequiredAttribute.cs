namespace SuperScrabble.CustomValidationAttributes.ResourceAttributes
{
    using SuperScrabble.LanguageResources;
    using System.ComponentModel.DataAnnotations;

    public class ResourceRequiredAttribute : RequiredAttribute
    {
        public ResourceRequiredAttribute(string errorMessageResourceName)
        {
            ErrorMessageResourceType = typeof(Resource);
            ErrorMessageResourceName = errorMessageResourceName;
        }
    }
}

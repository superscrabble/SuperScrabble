namespace SuperScrabble.CustomValidationAttributes
{
    using SuperScrabble.LanguageResources;
    using System.ComponentModel.DataAnnotations;

    public abstract class BaseResourceValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessageResourceType = typeof(Resource);
            return ValidationResult.Success;
        }
    }
}

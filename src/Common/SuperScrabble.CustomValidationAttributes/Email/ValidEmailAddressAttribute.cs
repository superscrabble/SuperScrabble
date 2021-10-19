namespace SuperScrabble.CustomValidationAttributes.Email
{
    using SuperScrabble.Common;
    using System.Text.RegularExpressions;
    using SuperScrabble.LanguageResources;
    using SuperScrabble.CustomValidationAttributes.Common;

    using System.ComponentModel.DataAnnotations;

    public class ValidEmailAddressAttribute : ResourceOriginValidationAttribute
    {
        private static readonly Regex regex = new(ModelValidationConstraints.Email.Pattern);

        public ValidEmailAddressAttribute() 
            : base(
                  typeof(string), 
                  nameof(Resource.EmailAddressIsInvalid))
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            => regex.IsMatch(value as string) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }
}

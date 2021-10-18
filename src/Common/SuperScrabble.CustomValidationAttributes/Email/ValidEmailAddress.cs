namespace SuperScrabble.CustomValidationAttributes.Email
{
    using SuperScrabble.CustomValidationAttributes.Common;
    using SuperScrabble.LanguageResources;

    using System.ComponentModel.DataAnnotations;

    public class ValidEmailAddress : ResourceOriginValidationAttribute
    {
        private readonly EmailAddressAttribute _emailAddressAttribute;

        public ValidEmailAddress() 
            : base(
                  typeof(string), 
                  nameof(Resource.EmailAddressIsInvalid))
        {
            _emailAddressAttribute = new();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            => _emailAddressAttribute.IsValid(value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }
}

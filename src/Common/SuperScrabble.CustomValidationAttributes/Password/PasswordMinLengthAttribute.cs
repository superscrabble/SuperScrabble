namespace SuperScrabble.CustomValidationAttributes.Password
{
    using SuperScrabble.LanguageResources;

    using System;
    using System.ComponentModel.DataAnnotations;

    using static SuperScrabble.Common.ModelValidationConstraints.Password;

    public class PasswordMinLengthAttribute : BaseResourceValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            base.IsValid(value);

            if (value is not string)
            {
                throw new ArgumentException("Property must be of type string");
            }

            ErrorMessageResourceType = typeof(Resource);
            ErrorMessageResourceName = nameof(Resource.PasswordIsTooShort);

            string password = value as string;

            if (password.Length < MinLength)
            {
                return new ValidationResult(string.Format(ErrorMessageString, MinLength));
            }

            return ValidationResult.Success;
        }
    }
}

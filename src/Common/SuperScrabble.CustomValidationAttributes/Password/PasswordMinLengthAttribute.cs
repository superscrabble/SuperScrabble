namespace SuperScrabble.CustomValidationAttributes.Password
{
    using SuperScrabble.LanguageResources;

    using System.ComponentModel.DataAnnotations;

    using static SuperScrabble.Common.ModelValidationConstraints.Password;

    public class PasswordMinLengthAttribute : BasePasswordValidationAttribute
    {
        public PasswordMinLengthAttribute() : base(nameof(Resource.PasswordIsTooShort))
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            base.IsValid(value, validationContext);

            string password = value as string;

            if (password.Length < MinLength)
            {
                return new ValidationResult(string.Format(ErrorMessageString, MinLength));
            }

            return ValidationResult.Success;
        }
    }
}

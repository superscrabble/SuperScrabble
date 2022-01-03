namespace SuperScrabble.Common.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    using SuperScrabble.WebApi.Resources;

    public class ValidEmailAddressAttribute : ValidationAttribute
    {
        private static readonly Regex regex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            bool isValid = false;

            if (value != null && value is string text)
            {
                isValid = regex.IsMatch(text);
            }

            return isValid ? ValidationResult.Success
                : new ValidationResult(UserValidationErrorCodes.InvalidEmail);
        }
    }
}

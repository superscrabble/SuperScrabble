namespace SuperScrabble.CustomValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PasswordMinLengthAttribute : ValidationAttribute
    {
        public PasswordMinLengthAttribute(int minLength)
        {
            MinLength = minLength;
        }

        private int MinLength { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not string)
            {
                throw new ArgumentException("Property must be of type string");
            }

            string password = value as string;

            if (password.Length < MinLength)
            {
                return new ValidationResult(string.Format(ErrorMessageString, MinLength));
            }

            return ValidationResult.Success;
        }
    }
}

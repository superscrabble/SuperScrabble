namespace SuperScrabble.CustomValidationAttributes
{
    using System;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;

    public abstract class RequireCharacterValidationAttribute : ValidationAttribute
    {
        public RequireCharacterValidationAttribute(bool isValueRequired, Func<char, bool> filter)
        {
            IsValueRequired = isValueRequired;
            Filter = filter;
        }

        private Func<char, bool> Filter { get; }

        private bool IsValueRequired { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!IsValueRequired)
            {
                return ValidationResult.Success;
            }

            if (value is not string)
            {
                throw new ArgumentException("Property type must be string");
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string text = value as string;

            if (text.Any(Filter))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}

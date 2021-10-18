namespace SuperScrabble.CustomValidationAttributes.SymbolRequiring
{
    using SuperScrabble.Common;

    using System;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using SuperScrabble.CustomValidationAttributes.Common;

    public abstract class RequireCharacterValidationAttribute : ResourceOriginValidationAttribute
    {
        private readonly bool _isValueRequired;
        private readonly Func<char, bool> _filter;

        public RequireCharacterValidationAttribute(bool isValueRequired, Func<char, bool> filter, string errorResourceName) 
            : base(typeof(string), errorResourceName)
        {
            _isValueRequired = isValueRequired;
            _filter = filter;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            base.IsValid(value, validationContext);

            if (!_isValueRequired)
            {
                return ValidationResult.Success;
            }

            Guard.Against.Null(value);

            string text = value as string;

            if (text.Any(_filter))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}

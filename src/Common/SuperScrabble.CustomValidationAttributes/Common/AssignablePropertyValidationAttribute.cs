namespace SuperScrabble.CustomValidationAttributes.Common
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public abstract class AssignablePropertyValidationAttribute : ValidationAttribute
    {
        private readonly Type _propertyType;

        protected AssignablePropertyValidationAttribute(Type propertyType)
        {
            _propertyType = propertyType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!value.GetType().IsAssignableFrom(_propertyType))
            {
                throw new ArgumentException($"Property must be assignable from {_propertyType}");
            }

            return ValidationResult.Success;
        }
    }
}

namespace SuperScrabble.CustomValidationAttributes.Common
{
    using System;
    using SuperScrabble.LanguageResources;

    public abstract class ResourceOriginValidationAttribute : AssignablePropertyValidationAttribute
    {
        protected ResourceOriginValidationAttribute(Type propertyType, string errorMessageResourceName) : base(propertyType)
        {
            ErrorMessageResourceType = typeof(Resource);
            ErrorMessageResourceName = errorMessageResourceName;
        }
    }
}

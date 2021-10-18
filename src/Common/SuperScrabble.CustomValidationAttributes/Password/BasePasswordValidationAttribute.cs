namespace SuperScrabble.CustomValidationAttributes.Password
{
    using SuperScrabble.CustomValidationAttributes.Common;

    public abstract class BasePasswordValidationAttribute : ResourceOriginValidationAttribute
    {
        protected BasePasswordValidationAttribute(string errorResourceName) 
            : base(
                  typeof(string), 
                  errorResourceName)
        {
        }
    }
}

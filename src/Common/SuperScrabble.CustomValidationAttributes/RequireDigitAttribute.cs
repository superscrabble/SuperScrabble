namespace SuperScrabble.CustomValidationAttributes
{
    public class RequireDigitAttribute : RequireCharacterValidationAttribute
    {
        public RequireDigitAttribute(bool isDigitRequired) : base(isDigitRequired, char.IsDigit)
        {
        }
    }
}

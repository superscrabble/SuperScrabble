namespace SuperScrabble.CustomValidationAttributes
{
    public class RequireUppercaseAttribute : RequireCharacterValidationAttribute
    {
        public RequireUppercaseAttribute(bool isUppercaseRequired) : base(isUppercaseRequired, char.IsUpper)
        {
        }
    }
}

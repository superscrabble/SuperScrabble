namespace SuperScrabble.CustomValidationAttributes
{
    public class RequireLowercaseAttribute : RequireCharacterValidationAttribute
    {
        public RequireLowercaseAttribute(bool isLowercaseRequired) : base(isLowercaseRequired, char.IsLower)
        {
        }
    }
}

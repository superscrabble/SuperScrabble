namespace SuperScrabble.CustomValidationAttributes
{
    using System.Collections.Generic;

    public class RequireNonAlphanumericAttribute : RequireCharacterValidationAttribute
    {
        static readonly HashSet<char> NonAlphanumericCharacters = new()
        {
            '!', '"', '#', '&', '%', 
            '$', '|', '(', ')', '*', 
            '+', ',', '-', '.', '/', 
            ':', ';', '<', '=', '>', 
            '?', '@', '[', '\\', ']', 
            '^', '_', '`', '{', '|', 
            '}', '~',
        };

        public RequireNonAlphanumericAttribute(bool isValueRequired) : base(isValueRequired, NonAlphanumericCharacters.Contains)
        {
        }
    }
}

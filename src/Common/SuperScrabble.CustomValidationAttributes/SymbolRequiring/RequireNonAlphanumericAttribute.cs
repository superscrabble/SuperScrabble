namespace SuperScrabble.CustomValidationAttributes.SymbolRequiring
{
    using SuperScrabble.Common;
    using SuperScrabble.LanguageResources;

    using System.Collections.Generic;

    public class RequireNonAlphanumericAttribute : RequireCharacterValidationAttribute
    {
        private static readonly HashSet<char> NonAlphanumericCharacters = new()
        {
            '!', '"', '#', '&', '%', '$', '|', '(', ')', '*', '+', 
            ',', '-', '.', '/', ':', ';', '<', '=', '>', '?', '@', 
            '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~',
        };

        public RequireNonAlphanumericAttribute()
            : base(
                  ModelValidationConstraints.Password.RequireNonAlphanumeric, 
                  NonAlphanumericCharacters.Contains, 
                  nameof(Resource.PasswordRequiresNonAlphanumeric))
        {
        }
    }
}

namespace SuperScrabble.CustomValidationAttributes.SymbolRequiring
{
    using SuperScrabble.Common;
    using SuperScrabble.LanguageResources;

    public class RequireLowercaseAttribute : RequireCharacterValidationAttribute
    {
        public RequireLowercaseAttribute() 
            : base(
                  ModelValidationConstraints.Password.RequireLowercase, 
                  char.IsLower, 
                  nameof(Resource.PasswordRequiresLowercase))
        {
        }
    }
}

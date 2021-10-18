namespace SuperScrabble.CustomValidationAttributes.SymbolRequiring
{
    using SuperScrabble.Common;
    using SuperScrabble.LanguageResources;

    public class RequireUppercaseAttribute : RequireCharacterValidationAttribute
    {
        public RequireUppercaseAttribute() 
            : base(
                  ModelValidationConstraints.Password.RequireUppercase, 
                  char.IsUpper,
                  nameof(Resource.PasswordRequiresUppercase))
        {
        }
    }
}

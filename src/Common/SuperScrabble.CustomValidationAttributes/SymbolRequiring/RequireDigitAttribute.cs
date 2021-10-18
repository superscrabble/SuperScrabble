namespace SuperScrabble.CustomValidationAttributes.SymbolRequiring
{
    using SuperScrabble.Common;
    using SuperScrabble.LanguageResources;

    public class RequireDigitAttribute : RequireCharacterValidationAttribute
    {
        public RequireDigitAttribute() 
            : base(
                  ModelValidationConstraints.Password.RequireDigit, 
                  char.IsDigit, 
                  nameof(Resource.PasswordRequiresDigit))
        {
        }
    }
}

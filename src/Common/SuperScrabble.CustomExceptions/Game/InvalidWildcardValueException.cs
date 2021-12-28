namespace SuperScrabble.CustomExceptions.Game
{
    using SuperScrabble.LanguageResources;

    public class InvalidWildcardValueException : ValidationFailedException
    {
        public InvalidWildcardValueException()
            : base(nameof(Resource.InvalidWildcardValue), Resource.InvalidWildcardValue)
        {
        }
    }
}

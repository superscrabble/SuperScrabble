namespace SuperScrabble.Common.Exceptions.Game
{
    public class InvalidWildcardValueException : GameValidationFailedException
    {
        public InvalidWildcardValueException() : base(InvalidWildcardValue)
        {
        }
    }
}

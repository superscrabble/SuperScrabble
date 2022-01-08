namespace SuperScrabble.Common.Exceptions.Game
{
    public class InvalidWildcardValueException : ValidationFailedException
    {
        public InvalidWildcardValueException() : base(InvalidWildcardValue)
        {
        }
    }
}

namespace SuperScrabble.Common.Exceptions
{
    public abstract class GameValidationFailedException : ValidationFailedException
    {
        protected GameValidationFailedException(string errorCode) : base(errorCode)
        {
        }
    }
}

namespace SuperScrabble.Common.Exceptions.Game
{
    public class ValidationFailedException : Exception
    {
        public ValidationFailedException(string errorCode)
        {
            this.ErrorCode = errorCode;
        }

        public string ErrorCode { get; }
    }
}

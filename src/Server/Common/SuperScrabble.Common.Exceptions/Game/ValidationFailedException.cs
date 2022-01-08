namespace SuperScrabble.Common.Exceptions.Game
{
    public abstract class ValidationFailedException : Exception
    {
        protected ValidationFailedException(string errorCode)
        {
            this.ErrorCode = errorCode;
        }

        public string ErrorCode { get; }
    }
}

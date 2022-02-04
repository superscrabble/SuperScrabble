namespace SuperScrabble.Common.Exceptions
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

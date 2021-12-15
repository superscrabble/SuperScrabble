namespace SuperScrabble.CustomExceptions.Game
{
    using System;

    public class ValidationFailedException : Exception
    {
        public ValidationFailedException(string errorCode, string errorMessage)
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }

        public string ErrorCode { get; }

        public string ErrorMessage { get; }
    }
}

namespace SuperScrabble.Common.Exceptions.Users
{
    public abstract class UserOperationFailedException : Exception
    {
        protected UserOperationFailedException(IEnumerable<string> errorCodes)
        {
            this.ErrorCodes = errorCodes;
        }

        public IEnumerable<string> ErrorCodes { get; }
    }
}

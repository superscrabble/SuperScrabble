namespace SuperScrabble.Common.Exceptions.Users
{
    public abstract class UserOperationFailedException : Exception
    {
        protected UserOperationFailedException(
            Dictionary<string, List<string>> propertyNamesByErrorCodes)
        {
            this.PropertyNamesByErrorCodes = propertyNamesByErrorCodes;
        }

        public Dictionary<string, List<string>> PropertyNamesByErrorCodes { get; }
    }
}

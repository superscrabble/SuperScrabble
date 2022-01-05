namespace SuperScrabble.Common.Exceptions.Users
{
    public abstract class UserOperationFailedException : Exception
    {
        protected UserOperationFailedException(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> propertyNamesByErrorCodes)
        {
            this.PropertyNamesByErrorCodes = propertyNamesByErrorCodes;
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> PropertyNamesByErrorCodes { get; }
    }
}

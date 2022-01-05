namespace SuperScrabble.Common.Exceptions.Users
{
    public class UserNotFoundException : UserOperationFailedException
    {
        public UserNotFoundException(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> propertyNamesByErrorCodes)
            : base(propertyNamesByErrorCodes)
        {
        }
    }
}

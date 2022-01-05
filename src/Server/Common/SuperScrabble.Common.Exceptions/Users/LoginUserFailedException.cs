namespace SuperScrabble.Common.Exceptions.Users
{
    public class LoginUserFailedException : UserOperationFailedException
    {
        public LoginUserFailedException(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> propertyNamesByErrorCodes)
            : base(propertyNamesByErrorCodes)
        {
        }
    }
}

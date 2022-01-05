namespace SuperScrabble.Common.Exceptions.Users
{
    public class RegisterUserFailedException : UserOperationFailedException
    {
        public RegisterUserFailedException(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> propertyNamesByErrorCodes)
            : base(propertyNamesByErrorCodes)
        {
        }
    }
}

namespace SuperScrabble.Common.Exceptions.Users
{
    public class RegisterUserFailedException : UserOperationFailedException
    {
        public RegisterUserFailedException(Dictionary<string, List<string>> propertyNamesByErrorCodes)
            : base(propertyNamesByErrorCodes)
        {
        }
    }
}

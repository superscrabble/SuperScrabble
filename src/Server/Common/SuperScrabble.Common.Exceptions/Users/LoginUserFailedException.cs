namespace SuperScrabble.Common.Exceptions.Users
{
    public class LoginUserFailedException : UserOperationFailedException
    {
        public LoginUserFailedException() : base(new())
        {
        }

        public LoginUserFailedException(Dictionary<string, List<string>> propertyNamesByErrorCodes)
            : base(propertyNamesByErrorCodes)
        {
        }
    }
}

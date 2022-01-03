namespace SuperScrabble.Common.Exceptions.Users
{
    public class LoginUserFailedException : UserOperationFailedException
    {
        public LoginUserFailedException(IEnumerable<string> errorCodes)
            : base(errorCodes)
        {
        }
    }
}

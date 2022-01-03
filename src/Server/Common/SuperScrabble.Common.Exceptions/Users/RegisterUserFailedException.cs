namespace SuperScrabble.Common.Exceptions.Users
{
    public class RegisterUserFailedException : UserOperationFailedException
    {
        public RegisterUserFailedException(IEnumerable<string> errorCodes)
            : base(errorCodes)
        {
        }
    }
}

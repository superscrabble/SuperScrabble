namespace SuperScrabble.Common.Exceptions.Users
{
    public class UserNotFoundException : UserOperationFailedException
    {
        public UserNotFoundException(IEnumerable<string> errorCodes)
            : base(errorCodes)
        {
        }
    }
}

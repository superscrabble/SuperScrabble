namespace SuperScrabble.Common.Exceptions.Users
{
    public class DeleteUserFailedException : UserOperationFailedException
    {
        public DeleteUserFailedException(IEnumerable<string> errorCodes)
            : base(errorCodes)
        {
        }
    }
}

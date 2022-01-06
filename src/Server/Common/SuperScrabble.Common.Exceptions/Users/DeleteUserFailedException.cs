namespace SuperScrabble.Common.Exceptions.Users
{
    public class DeleteUserFailedException : UserOperationFailedException
    {
        public DeleteUserFailedException(Dictionary<string, List<string>> propertyNamesByErrorCodes)
            : base(propertyNamesByErrorCodes)
        {
        }
    }
}

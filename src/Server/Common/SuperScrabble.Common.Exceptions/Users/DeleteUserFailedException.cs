namespace SuperScrabble.Common.Exceptions.Users
{
    public class DeleteUserFailedException : UserOperationFailedException
    {
        public DeleteUserFailedException(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> propertyNamesByErrorCodes)
            : base(propertyNamesByErrorCodes)
        {
        }
    }
}

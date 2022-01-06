namespace SuperScrabble.Common.Exceptions.Users
{
    public class UserNotFoundException : UserOperationFailedException
    {
        public UserNotFoundException(Dictionary<string, List<string>> propertyNamesByErrorCodes)
            : base(propertyNamesByErrorCodes)
        {
        }
    }
}

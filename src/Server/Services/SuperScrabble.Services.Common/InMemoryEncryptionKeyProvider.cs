namespace SuperScrabble.Services.Common
{
    public class InMemoryEncryptionKeyProvider : IEncryptionKeyProvider
    {
        public string GetEncryptionKey()
        {
            return "MY_SUPER_SECRET_KEY_ABC";
        }
    }
}

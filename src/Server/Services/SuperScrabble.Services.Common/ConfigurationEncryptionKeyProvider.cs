namespace SuperScrabble.Services.Common
{
    public class ConfigurationEncryptionKeyProvider : IEncryptionKeyProvider
    {
        // HS256 requires at least 128 bits; enforce a generous minimum.
        public const int MinKeyLength = 32;

        private readonly string encryptionKey;

        public ConfigurationEncryptionKeyProvider(string encryptionKey)
        {
            if (string.IsNullOrWhiteSpace(encryptionKey) || encryptionKey.Length < MinKeyLength)
            {
                throw new ArgumentException(
                    $"The JWT signing key must be at least {MinKeyLength} characters long. " +
                    "Set it via the 'Jwt:SigningKey' configuration value " +
                    "(appsettings, user-secrets, or the JWT__SIGNINGKEY environment variable).",
                    nameof(encryptionKey));
            }

            this.encryptionKey = encryptionKey;
        }

        public string GetEncryptionKey() => this.encryptionKey;
    }
}

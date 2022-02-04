namespace SuperScrabble.Services.Common
{
    using System.Text;
    using System.Security.Claims;
    using System.IdentityModel.Tokens.Jwt;

    using Microsoft.IdentityModel.Tokens;

    public class JsonWebTokenGenerator : IJsonWebTokenGenerator
    {
        private readonly IEncryptionKeyProvider encryptionKeyProvider;

        public JsonWebTokenGenerator(IEncryptionKeyProvider encryptionKeyProvider)
        {
            this.encryptionKeyProvider = encryptionKeyProvider;
        }

        public string GenerateToken(string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var keyBytes = Encoding.UTF8.GetBytes(
                this.encryptionKeyProvider.GetEncryptionKey());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userName)
                }),

                Expires = DateTime.UtcNow.AddDays(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

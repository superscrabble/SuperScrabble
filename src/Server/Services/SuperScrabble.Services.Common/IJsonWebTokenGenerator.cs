namespace SuperScrabble.Services.Common
{
    public interface IJsonWebTokenGenerator
    {
        string GenerateToken(string userName);
    }
}

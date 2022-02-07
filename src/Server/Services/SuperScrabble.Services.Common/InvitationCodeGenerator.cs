namespace SuperScrabble.Services.Common
{
    using System.Text;

    public class InvitationCodeGenerator : IInvitationCodeGenerator
    {
        public string GenerateInvitationCode()
        {
            var random = new Random();
            var codeBuilder = new StringBuilder();

            for (int i = 0; i < 4; i++)
            {
                char letter = (char)random.Next('A', 'Z' + 1);
                codeBuilder.Append(letter);
            }

            for (int i = 0; i < 3; i++)
            {
                int digit = random.Next(0, 10);
                codeBuilder.Append(digit);
            }

            return codeBuilder.ToString();
        }
    }
}

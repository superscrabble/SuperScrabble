namespace SuperScrabble.Common.Exceptions.Matchmaking
{
    public class UnexistingInvitationCodeException : MatchmakingFailedException
    {
        public UnexistingInvitationCodeException() : base(UnexistingInvitationCode)
        {
        }
    }
}

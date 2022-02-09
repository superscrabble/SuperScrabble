namespace SuperScrabble.Common.Exceptions.Matchmaking
{
    public class OnlyOwnerHasAccessException : MatchmakingFailedException
    {
        public OnlyOwnerHasAccessException() : base("OnlyOwnerHasAccess")
        {
        }
    }
}

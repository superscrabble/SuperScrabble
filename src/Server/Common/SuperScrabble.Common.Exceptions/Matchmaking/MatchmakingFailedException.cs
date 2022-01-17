namespace SuperScrabble.Common.Exceptions.Matchmaking
{
    public abstract class MatchmakingFailedException : ValidationFailedException
    {
        protected MatchmakingFailedException(string errorCode) : base(errorCode)
        {
        }
    }
}

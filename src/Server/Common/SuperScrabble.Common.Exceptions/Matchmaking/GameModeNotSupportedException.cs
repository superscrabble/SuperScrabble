namespace SuperScrabble.Common.Exceptions.Matchmaking
{
    public class GameModeNotSupportedException : MatchmakingFailedException
    {
        public GameModeNotSupportedException() : base(GameModeNotSupported)
        {
        }
    }
}

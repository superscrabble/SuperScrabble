namespace SuperScrabble.Common.Exceptions.Matchmaking
{
    public class PlayersPerGameCountNotSupportedException : MatchmakingFailedException
    {
        public PlayersPerGameCountNotSupportedException() : base(PlayersPerGameCountNotSupported)
        {
        }
    }
}

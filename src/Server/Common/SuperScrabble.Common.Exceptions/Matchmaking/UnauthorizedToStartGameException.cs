namespace SuperScrabble.Common.Exceptions.Matchmaking
{
    public class UnauthorizedToStartGameException : MatchmakingFailedException
    {
        public UnauthorizedToStartGameException() : base(UnauthorizedToStartGame)
        {
        }
    }
}

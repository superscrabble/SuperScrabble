namespace SuperScrabble.Common.Exceptions.Matchmaking
{
    public class GameLobbyFullException : MatchmakingFailedException
    {
        public GameLobbyFullException() : base(GameLobbyFull)
        {
        }
    }
}

namespace SuperScrabble.Common.Exceptions.Matchmaking
{
    public class PlayerAlreadyInsideLobbyException : MatchmakingFailedException
    {
        public PlayerAlreadyInsideLobbyException() : base(PlayerAlreadyInsideLobby)
        {
        }
    }
}

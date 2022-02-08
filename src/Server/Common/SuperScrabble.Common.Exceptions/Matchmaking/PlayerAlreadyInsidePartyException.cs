namespace SuperScrabble.Common.Exceptions.Matchmaking
{
    public class PlayerAlreadyInsidePartyException : MatchmakingFailedException
    {
        public PlayerAlreadyInsidePartyException() : base(PlayerAlreadyInsideLobby)
        {
        }
    }
}

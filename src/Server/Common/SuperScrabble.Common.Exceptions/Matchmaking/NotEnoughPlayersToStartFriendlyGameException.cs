namespace SuperScrabble.Common.Exceptions.Matchmaking
{
    public class NotEnoughPlayersToStartFriendlyGameException : MatchmakingFailedException
    {
        public NotEnoughPlayersToStartFriendlyGameException() : base(NotEnoughPlayersToStartFriendlyGame)
        {
        }
    }
}

namespace SuperScrabble.Common.Exceptions.Matchmaking
{
    public class NotEnoughPlayersToStartGameException : MatchmakingFailedException
    {
        public NotEnoughPlayersToStartGameException() : base(NotEnoughPlayersToStartFriendlyGame)
        {
        }
    }
}

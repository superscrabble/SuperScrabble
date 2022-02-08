namespace SuperScrabble.Common.Exceptions.Matchmaking.Party
{
    public class PartyNotFoundException : MatchmakingFailedException
    {
        public PartyNotFoundException() : base("PartyNotFound")
        {
        }
    }
}

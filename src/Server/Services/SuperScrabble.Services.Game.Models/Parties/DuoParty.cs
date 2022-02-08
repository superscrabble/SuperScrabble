namespace SuperScrabble.Services.Game.Models.Parties
{
    public class DuoParty : Party
    {
        public const int MinPlayersToStartGame = 2;
        public const int MaxAllowedPlayers = 2;

        public DuoParty(Member owner, string invitationCode)
            : base(owner, invitationCode, MinPlayersToStartGame, MaxAllowedPlayers)
        {
        }
    }
}

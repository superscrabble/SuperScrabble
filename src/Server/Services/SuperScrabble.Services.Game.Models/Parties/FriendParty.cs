namespace SuperScrabble.Services.Game.Models.Parties
{
    using SuperScrabble.Services.Game.Common.Enums;

    public class FriendParty : Party
    {
        public const int MinPlayersToStartGame = 2;
        public const int MaxAllowedPlayers = 4;

        public FriendParty(Member owner, string invitationCode)
            : base(owner, invitationCode, MinPlayersToStartGame, MaxAllowedPlayers)
        {
            this.TimerType = TimerType.Standard;
            this.TimerDifficulty = TimerDifficulty.Normal;
        }

        public TimerType TimerType { get; set; }

        public TimerDifficulty TimerDifficulty { get; set; }
    }
}

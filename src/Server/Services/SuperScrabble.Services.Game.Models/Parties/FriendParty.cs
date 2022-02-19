using SuperScrabble.Services.Game.Common.Enums;

namespace SuperScrabble.Services.Game.Models.Parties;

public class FriendParty : Party
{
    public const int MinPlayersToStartGame = 2;
    public const int MaxAllowedPlayers = 4;

    public FriendParty(Member owner, string id, string invitationCode)
        : base(owner, id, invitationCode, MinPlayersToStartGame, MaxAllowedPlayers)
    {
        TimerType = TimerType.Standard;
        TimerDifficulty = TimerDifficulty.Normal;
    }

    public TimerType TimerType { get; set; }

    public TimerDifficulty TimerDifficulty { get; set; }
}

namespace SuperScrabble.WebApi.ViewModels.Party
{
    using SuperScrabble.Services.Game.Common.Enums;

    public class FriendPartyConfig
    {
        public TimerType TimerType { get; set; }

        public TimerDifficulty TimerDifficulty { get; set; }
    }
}

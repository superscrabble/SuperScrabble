namespace SuperScrabble.WebApi.ViewModels.Party
{
    using SuperScrabble.Services.Game.Common.Enums;

    public class FriendPartyViewModel
    {
        public bool IsOwner { get; set; }

        public string Owner { get; set; } = default!;

        public string InvitationCode { get; set; } = default!;

        public PartyType PartyType { get; set; } = default!;

        public IEnumerable<string> Members { get; set; } = default!;

        public IEnumerable<ConfigSetting> ConfigSettings { get; set; } = default!;

        public bool IsPartyReady { get; set; }
    }
}

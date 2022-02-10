namespace SuperScrabble.WebApi.ViewModels.Party
{
    public class PlayerHasLeftPartyViewModel
    {
        public string LeaverUserName { get; set; } = default!;

        public IEnumerable<string> RemainingMembers { get; set; } = default!;

        public string Owner { get; set; } = default!;

        public bool IsOwner { get; set; } = default!;
    }
}

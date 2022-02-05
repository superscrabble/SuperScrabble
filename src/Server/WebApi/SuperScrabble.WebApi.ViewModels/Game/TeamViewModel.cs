namespace SuperScrabble.WebApi.ViewModels.Game
{
    public class TeamViewModel
    {
        public IEnumerable<PlayerViewModel> Players { get; set; } = default!;
    }
}

namespace SuperScrabble.WebApi.ViewModels.Games;

public class TeamViewModel
{
    public IEnumerable<PlayerViewModel> Players { get; set; } = default!;
}

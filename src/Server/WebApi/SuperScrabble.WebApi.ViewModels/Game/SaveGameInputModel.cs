namespace SuperScrabble.WebApi.ViewModels.Game
{
    using SuperScrabble.Services.Game.Models;

    public class SaveGameInputModel
    {
        public IEnumerable<Player> Players { get; set; } = default!;

        public string GameId { get; set; } = default!;
    }
}

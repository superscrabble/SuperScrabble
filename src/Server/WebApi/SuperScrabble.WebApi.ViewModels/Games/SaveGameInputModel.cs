using SuperScrabble.Services.Game.Models;

namespace SuperScrabble.WebApi.ViewModels.Games
{
    public class SaveGameInputModel
    {
        public IEnumerable<Player> Players { get; set; } = default!;

        public string GameId { get; set; } = default!;
    }
}

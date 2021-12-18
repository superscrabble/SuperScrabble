namespace SuperScrabble.InputModels.Game
{
    using System.Collections.Generic;

    using SuperScrabble.Services.Game.Models;

    public class SaveGameInputModel
    {
        public IEnumerable<Player> Players { get; set; }

        public string GameId { get; set; }
    }
}

namespace SuperScrabble.ViewModels
{
    using System.Collections.Generic;

    using SuperScrabble.Services.Game.Models;

    public class PlayerGameStateViewModel
    {
        public IEnumerable<Tile> Tiles { get; set; }

        public CommonGameStateViewModel CommonGameState { get; set; }
    }
}

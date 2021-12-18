namespace SuperScrabble.InputModels.Game
{
    using System.Collections.Generic;

    using SuperScrabble.Services.Game.Models;

    public class ExchangeTilesInputModel
    {
        public IEnumerable<Tile> TilesToExchange { get; set; }
    }
}

namespace SuperScrabble.InputModels.Game
{
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Models;

    public class WriteWordInputModel
    {
        public IEnumerable<KeyValuePair<Tile, Position>> PositionsByTiles { get; set; }
    }
}

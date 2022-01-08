namespace SuperScrabble.WebApi.ViewModels.Game
{
    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Common;

    public class WriteWordInputModel
    {
        public IEnumerable<KeyValuePair<Tile, Position>> PositionsByTiles { get; set; } = default!;
    }
}

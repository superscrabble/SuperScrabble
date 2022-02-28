namespace SuperScrabble.WebApi.ViewModels.Games
{
    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Common;

    public class WriteWordInputModel
    {
        public IEnumerable<KeyValuePair<Tile, Position>> PositionsByTiles { get; set; } = default!;
    }
}

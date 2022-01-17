namespace SuperScrabble.Services.Game.Models.Bags
{
    using SuperScrabble.Services.Game.Common;

    public interface IBag
    {
        int TilesCount { get; }

        Tile? DrawTile();

        IEnumerable<Tile> SwapTiles(IEnumerable<Tile> tiles);
    }
}

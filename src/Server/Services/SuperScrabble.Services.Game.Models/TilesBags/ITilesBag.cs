namespace SuperScrabble.Services.Game.Models.TilesBags
{
    using SuperScrabble.Services.Game.Common;

    public interface ITilesBag
    {
        int TilesCount { get; }

        Tile? DrawTile();

        void AddTiles(IEnumerable<Tile> tiles);
    }
}

namespace SuperScrabble.Services.Game.Common.TilesProviders
{
    public interface ITilesProvider
    {
        IEnumerable<KeyValuePair<char, TileInfo>> GetTiles();

        IEnumerable<Tile> GetAllWildcardOptions();
    }
}

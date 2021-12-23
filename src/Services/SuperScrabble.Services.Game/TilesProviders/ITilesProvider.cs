namespace SuperScrabble.Services.Game.TilesProviders
{
    using System.Collections.Generic;

    using SuperScrabble.Services.Game.Models;

    public interface ITilesProvider
    {
        IEnumerable<KeyValuePair<char, TileInfo>> GetTiles();

        IEnumerable<Tile> GetAllWildcardOptions();
    }
}

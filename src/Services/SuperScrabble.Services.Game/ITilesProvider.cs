namespace SuperScrabble.Services.Game
{
    using System.Collections.Generic;

    public interface ITilesProvider
    {
        IEnumerable<KeyValuePair<char, TileInfo>> GetTiles();
    }
}

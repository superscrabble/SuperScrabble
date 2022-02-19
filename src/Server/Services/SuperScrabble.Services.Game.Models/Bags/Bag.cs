using SuperScrabble.Services.Game.Common;
using SuperScrabble.Services.Game.Common.TilesProviders;

namespace SuperScrabble.Services.Game.Models.Bags;

public class Bag : IBag
{
    private readonly List<Tile> _tiles = new();

    public Bag(ITilesProvider tilesProvider)
    {
        InitializeTiles(tilesProvider);
    }

    public int TilesCount => _tiles.Count;

    public Tile? DrawTile()
    {
        if (TilesCount <= 0)
        {
            return null;
        }

        var random = new Random();
        int index = random.Next(TilesCount);

        Tile tile = _tiles[index];
        _tiles.RemoveAt(index);

        return tile;
    }

    public IEnumerable<Tile> SwapTiles(IEnumerable<Tile> tiles)
    {
        var drawnTiles = new List<Tile>();

        for (int i = 0; i < tiles.Count(); i++)
        {
            Tile? drawnTile = DrawTile();

            if (drawnTile == null)
            {
                break;
            }

            drawnTiles.Add(drawnTile);
        }

        _tiles.AddRange(tiles);
        return drawnTiles;
    }

    private void InitializeTiles(ITilesProvider tilesProvider)
    {
        foreach (var tileInfoByLetter in tilesProvider.GetTiles())
        {
            for (int i = 0; i < tileInfoByLetter.Value.Count; i++)
            {
                var tile = new Tile(tileInfoByLetter.Key, tileInfoByLetter.Value.Points);
                _tiles.Add(tile);
            }
        }
    }
}

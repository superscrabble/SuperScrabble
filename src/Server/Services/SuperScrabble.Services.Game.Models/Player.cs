using SuperScrabble.Services.Game.Common;

namespace SuperScrabble.Services.Game.Models;

public class Player
{
    private readonly List<Tile> _tiles = new();

    public Player(string userName, string? connectionId)
    {
        Points = 0;
        UserName = userName;
        ConnectionId = connectionId;
        ConsecutiveSkipsCount = 0;
        HasLeftTheGame = false;
    }

    public int Points { get; set; }

    public string UserName { get; }

    public string? ConnectionId { get; set; }

    public int ConsecutiveSkipsCount { get; set; }

    public bool HasLeftTheGame { get; private set; }

    public IReadOnlyCollection<Tile> Tiles => _tiles.AsReadOnly();

    public int TilesPointsSum => _tiles.Sum(tile => tile.Points);

    public void LeaveGame()
    {
        HasLeftTheGame = true;
    }

    public void AddTile(Tile tile)
    {
        _tiles.Add(tile);
    }

    public void AddTiles(IEnumerable<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            AddTile(tile);
        }
    }

    public void RemoveTile(Tile tile)
    {
        var tileToRemove = _tiles.FirstOrDefault(playerTile => playerTile.Equals(tile)
            || (tile?.Points == 0 && playerTile.IsWildcard));

        if (tileToRemove != null)
        {
            _tiles.Remove(tileToRemove);
        }
    }

    public void RemoveTiles(IEnumerable<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            RemoveTile(tile);
        }
    }
}

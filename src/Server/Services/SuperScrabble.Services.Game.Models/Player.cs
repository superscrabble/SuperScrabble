namespace SuperScrabble.Services.Game.Models
{
    using SuperScrabble.Services.Game.Common;

    public class Player
    {
        private readonly List<Tile> tiles = new();

        public Player(string userName, string connectionId)
        {
            this.Points = 0;
            this.UserName = userName;
            this.ConnectionId = connectionId;
            this.ConsecutiveSkipsCount = 0;
            this.HasLeftTheGame = false;
        }

        public int Points { get; set; }

        public string UserName { get; }

        public string ConnectionId { get; set; }

        public int ConsecutiveSkipsCount { get; set; }

        public bool HasLeftTheGame { get; private set; }

        public IReadOnlyCollection<Tile> Tiles => this.tiles.AsReadOnly();

        public int TilesPointsSum => this.Tiles.Sum(tile => tile.Points);

        public void LeaveGame()
        {
            this.HasLeftTheGame = true;
        }

        public void AddTile(Tile tile)
        {
            this.tiles.Add(tile);
        }

        public void RemoveTile(Tile tile)
        {
            Tile? tileToRemove = this.tiles.FirstOrDefault(playerTile
                => playerTile.Equals(tile) || (tile?.Points == 0 && playerTile.IsWildcard));

            if (tileToRemove != null)
            {
                this.tiles.Remove(tileToRemove);
            }
        }

        public void RemoveTiles(IEnumerable<Tile> tiles)
        {
            foreach (Tile tile in tiles)
            {
                this.RemoveTile(tile);
            }
        }

        public void AddTiles(IEnumerable<Tile> tiles)
        {
            foreach (Tile tile in tiles)
            {
                this.AddTile(tile);
            }
        }
    }
}

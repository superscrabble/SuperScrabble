namespace SuperScrabble.Services.Game.Models
{
    using System.Linq;
    using System.Collections.Generic;

    public class Player
    {
        private readonly List<Tile> tiles = new();
        private readonly IGameplayConstantsProvider gameplayConstantsProvider;

        public Player(string userName, int points, string connectionId, IGameplayConstantsProvider gameplayConstantsProvider)
        {
            this.UserName = userName;
            this.Points = points;
            this.ConnectionId = connectionId;
            this.gameplayConstantsProvider = gameplayConstantsProvider;
            this.ConsecutiveSkipsCount = 0;
            this.HasLeftTheGame = false;
        }

        public int ConsecutiveSkipsCount { get; set; }

        public string UserName { get; set; }

        public int Points { get; set; }

        public string ConnectionId { get; set; }

        public bool HasLeftTheGame { get; private set; }

        public IReadOnlyCollection<Tile> Tiles => this.tiles.AsReadOnly();

        public void SubtractRemainingTilesPoints()
        {
            int pointsToSubtract = 0;

            foreach (Tile tile in this.Tiles)
            {
                pointsToSubtract += tile.Points;
            }

            this.Points -= pointsToSubtract;
        }

        public void AddTile(Tile tile)
        {
            this.tiles.Add(tile);
        }

        public void RemoveTile(Tile tile)
        {
            Tile tileToRemove = this.tiles.FirstOrDefault(playerTile => 
                playerTile.Equals(tile) || (tile?.Points == 0
                && playerTile.Letter == this.gameplayConstantsProvider.WildcardValue
                && playerTile.Points == 0));

            if (tileToRemove != null)
            {
                this.tiles.Remove(tileToRemove);
            }
        }

        public Tile GetTile(int index)
        {
            if (index < 0 || index >= this.tiles.Count)
            {
                return null;
            }

            return this.tiles[index];
        }

        public void LeaveGame()
        {
            this.HasLeftTheGame = true;
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

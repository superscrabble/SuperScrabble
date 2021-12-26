namespace SuperScrabble.Services.Game.Models
{
    using System.Collections.Generic;

    public class Player
    {
        private readonly List<Tile> tiles = new();

        public Player(string userName, int points, string connectionId)
        {
            this.UserName = userName;
            this.Points = points;
            this.ConnectionId = connectionId;
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
            this.tiles.Remove(tile);
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
    }
}

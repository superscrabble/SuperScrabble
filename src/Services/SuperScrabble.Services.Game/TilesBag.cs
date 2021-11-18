namespace SuperScrabble.Services.Game
{
    using System;
    using System.Collections.Generic;

    public class TilesBag
    {
        private readonly List<Tile> tiles = new();

        public TilesBag(IEnumerable<KeyValuePair<char, TileInfo>> tiles)
        {
            foreach (var tile in tiles)
            {
                for (int i = 0; i < tile.Value.Count; i++)
                {
                    this.tiles.Add(new Tile(tile.Key, tile.Value.Points));
                }
            }
        }

        public int TilesCount => this.tiles.Count;

        public Tile GetTile()
        {
            if (this.TilesCount <= 0)
            {
                return null;
            }

            var random = new Random();
            int index = random.Next(this.TilesCount);

            return this.tiles[index];
        }
    }
}
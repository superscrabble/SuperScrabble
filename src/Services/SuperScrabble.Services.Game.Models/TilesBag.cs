namespace SuperScrabble.Services.Game.Models
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

        public Tile DrawTile()
        {
            if (this.TilesCount <= 0)
            {
                return null;
            }

            var random = new Random();
            int index = random.Next(this.TilesCount);

            var tile = this.tiles[index];
            this.tiles.RemoveAt(index);

            return tile;
        }

        public void AddTiles(IEnumerable<Tile> tiles)
        {
            this.tiles.AddRange(tiles);
        }
    }
}
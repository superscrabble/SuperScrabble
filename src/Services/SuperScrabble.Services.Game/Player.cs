using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.Services.Game
{
    public class Player
    {
        private readonly List<Tile> tiles = new();

        public string UserName { get; set; }

        public int Points { get; set; }

        public IReadOnlyCollection<Tile> Tiles => this.tiles.AsReadOnly();

        public void AddTile(Tile tile)
        {
            tiles.Add(tile);
        }

        public void RemoveTile(Tile tile)
        {
            tiles.Remove(tile);
        }

        public Tile GetTile(int index)
        {
            if (index < 0 || index >= tiles.Count)
            {
                return null;
            }
            return tiles[index];
        }
    }
}

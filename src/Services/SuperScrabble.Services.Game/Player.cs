using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.Services.Game
{
    class Player
    {
        public string UserName { get; set; }
        public int Points { get; set; }
        public IList<Tile> Tiles { get; set; }

        public void AddTile(Tile tile)
        {
            Tiles.Add(tile);
        }

        public void RemoveTile(Tile tile)
        {
            Tiles.Remove(tile);
        }
    }
}

using System;

namespace SuperScrabble.Services.Game
{
    public class Tile
    {
        public char Letter { get; set; }
        public int Points { get; set; }

        public override bool Equals(object obj)
        {
            if(obj != null && obj is Tile)
            {
                Tile other = obj as Tile;
                return other.Letter == this.Letter && other.Points == this.Points;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Letter, Points);
        }
    }
}
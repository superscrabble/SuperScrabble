namespace SuperScrabble.Services.Game.Models
{
    using System;

    public class Tile
    {
        public Tile(char letter, int points)
        {
            this.Letter = letter;
            this.Points = points;
        }

        public char Letter { get; set; }

        public int Points { get; set; }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Tile)
            {
                Tile other = obj as Tile;
                return this.Letter == other.Letter && this.Points == other.Points;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Letter, this.Points);
        }
    }
}
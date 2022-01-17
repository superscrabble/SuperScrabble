namespace SuperScrabble.Services.Game.Common
{
    using SuperScrabble.Common;

    public class Tile
    {
        public Tile(char letter, int points)
        {
            this.Letter = letter;
            this.Points = points;
        }

        public char Letter { get; set; }

        public int Points { get; set; }

        public bool IsWildcard =>
            this.Letter == GlobalConstants.WildcardValue && this.Points == 0;

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is Tile other)
            {
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

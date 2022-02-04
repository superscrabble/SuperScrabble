namespace SuperScrabble.Common
{
    public class Position
    {
        public Position(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

        public int Row { get; set; }

        public int Column { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is Position other)
            {
                return other.Row == this.Row && other.Column == this.Column;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Row, this.Column);
        }
    }
}

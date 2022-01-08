namespace SuperScrabble.Common.Exceptions.Game
{
    public class TilePositionOutsideBoardRangeException : ValidationFailedException
    {
        public TilePositionOutsideBoardRangeException() : base(TilePositionOutsideBoardRange)
        {
        }
    }
}

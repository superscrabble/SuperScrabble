namespace SuperScrabble.Common.Exceptions.Game
{
    public class TilePositionOutsideBoardRangeException : GameValidationFailedException
    {
        public TilePositionOutsideBoardRangeException() : base(TilePositionOutsideBoardRange)
        {
        }
    }
}

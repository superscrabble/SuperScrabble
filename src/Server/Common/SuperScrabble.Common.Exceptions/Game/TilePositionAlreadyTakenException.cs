namespace SuperScrabble.Common.Exceptions.Game
{
    public class TilePositionAlreadyTakenException : ValidationFailedException
    {
        public TilePositionAlreadyTakenException() : base(TilePositionAlreadyTaken)
        {
        }
    }
}

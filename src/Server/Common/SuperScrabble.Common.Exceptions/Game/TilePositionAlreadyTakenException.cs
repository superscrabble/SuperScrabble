namespace SuperScrabble.Common.Exceptions.Game
{
    public class TilePositionAlreadyTakenException : GameValidationFailedException
    {
        public TilePositionAlreadyTakenException() : base(TilePositionAlreadyTaken)
        {
        }
    }
}

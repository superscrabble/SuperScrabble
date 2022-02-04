namespace SuperScrabble.Common.Exceptions.Game
{
    public class InputTilesPositionsCollideException : GameValidationFailedException
    {
        public InputTilesPositionsCollideException() : base(InputTilesPositionsCollide)
        {
        }
    }
}

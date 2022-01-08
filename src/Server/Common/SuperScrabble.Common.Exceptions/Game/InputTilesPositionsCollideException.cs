namespace SuperScrabble.Common.Exceptions.Game
{
    public class InputTilesPositionsCollideException : ValidationFailedException
    {
        public InputTilesPositionsCollideException() : base(InputTilesPositionsCollide)
        {
        }
    }
}

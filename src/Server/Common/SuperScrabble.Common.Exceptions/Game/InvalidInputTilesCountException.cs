namespace SuperScrabble.Common.Exceptions.Game
{
    public class InvalidInputTilesCountException : ValidationFailedException
    {
        public InvalidInputTilesCountException() : base(InvalidInputTilesCount)
        {
        }
    }
}

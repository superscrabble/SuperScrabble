namespace SuperScrabble.Common.Exceptions.Game
{
    public class InvalidInputTilesCountException : GameValidationFailedException
    {
        public InvalidInputTilesCountException() : base(InvalidInputTilesCount)
        {
        }
    }
}

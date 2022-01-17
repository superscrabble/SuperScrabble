namespace SuperScrabble.Common.Exceptions.Game
{
    public class TilesNotOnTheSameLineException : GameValidationFailedException
    {
        public TilesNotOnTheSameLineException() : base(TilesNotOnTheSameLine)
        {
        }
    }
}

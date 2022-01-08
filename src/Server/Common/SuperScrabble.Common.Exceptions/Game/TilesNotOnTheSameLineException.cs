namespace SuperScrabble.Common.Exceptions.Game
{
    public class TilesNotOnTheSameLineException : ValidationFailedException
    {
        public TilesNotOnTheSameLineException() : base(TilesNotOnTheSameLine)
        {
        }
    }
}

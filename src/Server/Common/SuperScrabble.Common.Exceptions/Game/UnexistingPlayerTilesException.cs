namespace SuperScrabble.Common.Exceptions.Game
{
    public class UnexistingPlayerTilesException : ValidationFailedException
    {
        public UnexistingPlayerTilesException() : base(UnexistingPlayerTiles)
        {
        }
    }
}

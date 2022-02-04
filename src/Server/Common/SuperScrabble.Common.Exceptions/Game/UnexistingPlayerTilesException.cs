namespace SuperScrabble.Common.Exceptions.Game
{
    public class UnexistingPlayerTilesException : GameValidationFailedException
    {
        public UnexistingPlayerTilesException() : base(UnexistingPlayerTiles)
        {
        }
    }
}

namespace SuperScrabble.CustomExceptions.Game
{
    using SuperScrabble.LanguageResources;

    public class UnexistingPlayerTileException : ValidationFailedException
    {
        public UnexistingPlayerTileException()
            : base(nameof(Resource.UnexistingPlayerTile), Resource.UnexistingPlayerTile)
        {
        }
    }
}

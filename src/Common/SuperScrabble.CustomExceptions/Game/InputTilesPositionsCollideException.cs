namespace SuperScrabble.CustomExceptions.Game
{
    using SuperScrabble.LanguageResources;

    public class InputTilesPositionsCollideException : ValidationFailedException
    {
        public InputTilesPositionsCollideException()
            : base(nameof(Resource.InputTilesPositionsCollide), Resource.InputTilesPositionsCollide)
        {
        }
    }
}

namespace SuperScrabble.CustomExceptions.Game
{
    using SuperScrabble.LanguageResources;

    public class TilePositionAlreadyTakenException : ValidationFailedException
    {
        public TilePositionAlreadyTakenException()
            : base(nameof(Resource.TilePositionAlreadyTaken), Resource.TilePositionAlreadyTaken)
        {
        }
    }
}

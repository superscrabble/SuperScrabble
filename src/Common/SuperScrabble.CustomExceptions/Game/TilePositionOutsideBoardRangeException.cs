namespace SuperScrabble.CustomExceptions.Game
{
    using SuperScrabble.LanguageResources;

    public class TilePositionOutsideBoardRangeException : ValidationFailedException
    {
        public TilePositionOutsideBoardRangeException()
            : base(nameof(Resource.TilePositionOutsideBoardRange), Resource.TilePositionOutsideBoardRange)
        {
        }
    }
}

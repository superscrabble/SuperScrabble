namespace SuperScrabble.CustomExceptions.Game
{
    using SuperScrabble.LanguageResources;

    public class InvalidInputTilesCountException : ValidationFailedException
    {
        public InvalidInputTilesCountException()
            : base(nameof(Resource.InvalidInputTilesCount), Resource.InvalidInputTilesCount)
        {
        }
    }
}

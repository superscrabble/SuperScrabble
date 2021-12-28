namespace SuperScrabble.CustomExceptions.Game
{
    using SuperScrabble.LanguageResources;

    public class GapsBetweenInputTilesNotAllowedException : ValidationFailedAfterInputTilesHaveBeenPlacedException
    {
        public GapsBetweenInputTilesNotAllowedException()
            : base(nameof(Resource.GapsBetweenInputTilesNotAllowed), Resource.GapsBetweenInputTilesNotAllowed)
        {
        }
    }
}

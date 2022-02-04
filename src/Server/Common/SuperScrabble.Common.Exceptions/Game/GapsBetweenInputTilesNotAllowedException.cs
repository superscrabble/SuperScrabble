namespace SuperScrabble.Common.Exceptions.Game
{
    public class GapsBetweenInputTilesNotAllowedException : ValidationFailedAfterInputTilesHaveBeenPlacedException
    {
        public GapsBetweenInputTilesNotAllowedException() : base(GapsBetweenInputTilesNotAllowed)
        {
        }
    }
}

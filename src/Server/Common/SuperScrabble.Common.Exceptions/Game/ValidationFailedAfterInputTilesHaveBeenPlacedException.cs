namespace SuperScrabble.Common.Exceptions.Game
{
    public abstract class ValidationFailedAfterInputTilesHaveBeenPlacedException : ValidationFailedException
    {
        protected ValidationFailedAfterInputTilesHaveBeenPlacedException(string errorCode) : base(errorCode)
        {
        }
    }
}

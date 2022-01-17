namespace SuperScrabble.Common.Exceptions.Game
{
    public abstract class ValidationFailedAfterInputTilesHaveBeenPlacedException : GameValidationFailedException
    {
        protected ValidationFailedAfterInputTilesHaveBeenPlacedException(string errorCode) : base(errorCode)
        {
        }
    }
}

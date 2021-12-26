namespace SuperScrabble.CustomExceptions.Game
{
    public class ValidationFailedAfterInputTilesHaveBeenPlacedException : ValidationFailedException
    {
        public ValidationFailedAfterInputTilesHaveBeenPlacedException(string errorCode, string errorMessage)
            : base(errorCode, errorMessage)
        {
        }
    }
}

namespace SuperScrabble.CustomExceptions.Game
{
    using SuperScrabble.LanguageResources;

    public class TilesNotOnTheSameLineException : ValidationFailedException
    {
        public TilesNotOnTheSameLineException()
            : base(nameof(Resource.TilesNotOnTheSameLine), Resource.TilesNotOnTheSameLine)
        {
        }
    }
}

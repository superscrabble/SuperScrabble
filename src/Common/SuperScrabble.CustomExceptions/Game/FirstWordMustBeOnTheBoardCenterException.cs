namespace SuperScrabble.CustomExceptions.Game
{
    using SuperScrabble.LanguageResources;

    public class FirstWordMustBeOnTheBoardCenterException : ValidationFailedException
    {
        public FirstWordMustBeOnTheBoardCenterException()
            : base(nameof(Resource.FirstWordMustBeOnTheBoardCenter), Resource.FirstWordMustBeOnTheBoardCenter)
        {
        }
    }
}

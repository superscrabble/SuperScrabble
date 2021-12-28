namespace SuperScrabble.CustomExceptions.Game
{
    using SuperScrabble.LanguageResources;

    public class PlayerNotOnTurnException : ValidationFailedException
    {
        public PlayerNotOnTurnException()
            : base(nameof(Resource.TheGivenPlayerIsNotOnTurn), Resource.TheGivenPlayerIsNotOnTurn)
        {
        }
    }
}

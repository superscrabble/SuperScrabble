namespace SuperScrabble.Common.Exceptions.Game
{
    public class PlayerNotOnTurnException : GameValidationFailedException
    {
        public PlayerNotOnTurnException() : base(PlayerIsNotOnTurn)
        {
        }
    }
}

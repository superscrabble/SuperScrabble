namespace SuperScrabble.Common.Exceptions.Game
{
    public class PlayerNotOnTurnException : ValidationFailedException
    {
        public PlayerNotOnTurnException() : base(PlayerIsNotOnTurn)
        {
        }
    }
}

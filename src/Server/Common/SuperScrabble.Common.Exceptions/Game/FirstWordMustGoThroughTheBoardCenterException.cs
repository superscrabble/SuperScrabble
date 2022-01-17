namespace SuperScrabble.Common.Exceptions.Game
{
    public class FirstWordMustGoThroughTheBoardCenterException : GameValidationFailedException
    {
        public FirstWordMustGoThroughTheBoardCenterException() : base(FirstWordMustGoThroughTheBoardCenter)
        {
        }
    }
}

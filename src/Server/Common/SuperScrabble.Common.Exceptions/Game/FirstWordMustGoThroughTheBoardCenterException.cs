namespace SuperScrabble.Common.Exceptions.Game
{
    public class FirstWordMustGoThroughTheBoardCenterException : ValidationFailedException
    {
        public FirstWordMustGoThroughTheBoardCenterException() : base(FirstWordMustGoThroughTheBoardCenter)
        {
        }
    }
}

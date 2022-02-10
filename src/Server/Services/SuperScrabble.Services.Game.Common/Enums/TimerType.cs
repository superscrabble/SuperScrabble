namespace SuperScrabble.Services.Game.Common.Enums
{
    public enum TimerType
    {
        Standard = 1,
        Chess = 2,
    }

    public static class TimerTypeExtensions
    {
        public static bool IsDefault(this TimerType timerType)
        {
            return timerType == TimerType.Standard;
        }
    }
}

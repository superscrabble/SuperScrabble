namespace SuperScrabble.Services.Game.Common.Enums
{
    public enum TimerDifficulty
    {
        Slow = 1,
        Normal = 2,
        Fast = 3,
    }

    public static class TimerDifficultyExtensions
    {
        private static readonly Dictionary<TimerDifficulty, int> standardTimerSeconds = new();
        private static readonly Dictionary<TimerDifficulty, int> chessTimerSeconds = new();

        public static int GetSeconds(this TimerDifficulty timerDifficulty, TimerType timerType)
        {
            switch (timerType)
            {
                case TimerType.Standard:
                    return standardTimerSeconds[timerDifficulty];

                case TimerType.Chess:
                    return chessTimerSeconds[timerDifficulty];

                default:
                    throw new NotSupportedException($"Not supported {nameof(TimerType)} enum value.");
            }
        }
    }
}

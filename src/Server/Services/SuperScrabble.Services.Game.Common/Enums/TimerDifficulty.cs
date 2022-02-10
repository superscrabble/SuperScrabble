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
        private static readonly Dictionary<TimerDifficulty, int> standardTimerSeconds = new()
        {
            [TimerDifficulty.Slow] = 120,
            [TimerDifficulty.Normal] = 80,
            [TimerDifficulty.Fast] = 60,
        };

        private static readonly Dictionary<TimerDifficulty, int> chessTimerSeconds = new()
        {
            [TimerDifficulty.Slow] = 1200,
            [TimerDifficulty.Normal] = 800,
            [TimerDifficulty.Fast] = 600,
        };

        public static int GetSeconds(this TimerDifficulty timerDifficulty, TimerType timerType)
        {
            return timerType switch
            {
                TimerType.Standard => standardTimerSeconds[timerDifficulty],
                TimerType.Chess => chessTimerSeconds[timerDifficulty],
                _ => throw new NotSupportedException($"Not supported {nameof(TimerType)} enum value."),
            };
        }

        public static bool IsDefault(this TimerDifficulty timerDifficulty)
        {
            return timerDifficulty == TimerDifficulty.Normal;
        }
    }
}

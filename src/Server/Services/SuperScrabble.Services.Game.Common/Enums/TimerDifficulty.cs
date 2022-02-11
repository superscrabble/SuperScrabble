namespace SuperScrabble.Services.Game.Common.Enums
{
    public enum TimerDifficulty
    {
        Fast = 1,
        Normal = 2,
        Slow = 3,
        ExtraSlow = 4
    }

    public static class TimerDifficultyExtensions
    {
        private static readonly Dictionary<TimerDifficulty, int> standardTimerSeconds = new()
        {
            [TimerDifficulty.Slow] = 120,
            [TimerDifficulty.Normal] = 90,
            [TimerDifficulty.Fast] = 60,
            [TimerDifficulty.ExtraSlow] = 180,
        };

        private static readonly Dictionary<TimerDifficulty, int> chessTimerSeconds = new()
        {
            [TimerDifficulty.Slow] = 1200,
            [TimerDifficulty.Normal] = 900,
            [TimerDifficulty.Fast] = 600,
            [TimerDifficulty.ExtraSlow] = 1500,
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

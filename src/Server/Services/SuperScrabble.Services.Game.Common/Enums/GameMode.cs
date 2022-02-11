namespace SuperScrabble.Services.Game.Common.Enums
{
    public enum GameMode
    {
        Duel = 1,
        Duo = 2,
        Classic = 3,
        ChessScrabble = 4,
        SuperScrabble = 5,
        MadBoards = 6,
    }

    public static class GameModeExtensions
    {
        private static readonly Dictionary<GameMode, int> teamsCountsByGameModes = new()
        {
            [GameMode.Duel] = 2,
            [GameMode.Duo] = 2,
            [GameMode.Classic] = 4,
            [GameMode.ChessScrabble] = 2,
            [GameMode.SuperScrabble] = 4,
            [GameMode.MadBoards] = 2,
        };

        public static int GetTeamsCount(this GameMode gameMode)
        {
            if (!teamsCountsByGameModes.ContainsKey(gameMode))
            {
                throw new NotSupportedException($"Not supported {nameof(GameMode)} enum value.");
            }

            return teamsCountsByGameModes[gameMode];
        }
    }
}

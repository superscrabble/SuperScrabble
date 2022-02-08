namespace SuperScrabble.Services.Game.Common.Enums
{
    public enum GameMode
    {
        OneVsOne = 1,
        TwoVsTwo = 2,
        ChessScrabble = 3,
        ClassicScrabble = 4,
        SuperScrabble = 5,
        MadBoards = 6,
        Friends = 7,
        Random = 8,
        Custom = 9,
        Duo = 10,
    }

    public static class GameModeExtensions
    {
        public static int GetTeamsCount(this GameMode gameMode)
        {
            if (gameMode == GameMode.Duo)
            {
                return 2;
            }

            return 2;
        }
    }
}

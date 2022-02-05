namespace SuperScrabble.Services.Game.Common.GameplayConstantsProviders
{
    public class StandardGameplayConstantsProvider : IGameplayConstantsProvider
    {
        public StandardGameplayConstantsProvider(int playersPerGameCount, int gameTimerSeconds)
        {
            this.PlayersPerGameCount = playersPerGameCount;
            this.GameTimerSeconds = gameTimerSeconds;
        }

        public int PlayersPerGameCount { get; }

        public int GameTimerSeconds { get; }

        public int PlayerTilesCount => 7;

        public int MinSkipsCountForEachPlayerToEndTheGame => 2;

        public int BonusPointsForUsingAllTiles => 50;

        public int MinWordLettersCount => 2;
    }
}

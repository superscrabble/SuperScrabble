namespace SuperScrabble.Services.Game.Common.GameplayConstantsProviders
{
    public interface IGameplayConstantsProvider
    {
        int PlayerTilesCount { get; }

        int PlayersPerGameCount { get; }

        int MinSkipsCountForEachPlayerToEndTheGame { get; }

        int BonusPointsForUsingAllTiles { get; }

        int GameTimerSeconds { get; }

        char WildcardValue { get; }

        int MinWordLettersCount { get; }
    }
}

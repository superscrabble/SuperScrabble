namespace SuperScrabble.Services.Game
{
    public interface IGameplayConstantsProvider
    {
        int PlayerTilesCount { get; }

        int PlayersPerGameCount { get; }

        int MinSkipsCountForEachPlayerToEndTheGame { get; }

        int BonusPointsForUsingAllTiles { get; }

        char WildcardValue { get; }

        int MinConsecutiveSkipsCountToEndTheGame { get; }
    }
}

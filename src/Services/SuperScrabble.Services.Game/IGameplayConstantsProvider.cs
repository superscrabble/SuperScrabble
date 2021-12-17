namespace SuperScrabble.Services.Game
{
    public interface IGameplayConstantsProvider
    {
        int PlayerTilesCount { get; }

        int PlayersPerGameCount { get; }
    }
}

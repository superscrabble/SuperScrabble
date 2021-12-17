namespace SuperScrabble.Services.Game
{
    public class StandardGameplayConstantsProvider : IGameplayConstantsProvider
    {
        public int PlayerTilesCount => 7;

        public int PlayersPerGameCount => 2;
    }
}

namespace SuperScrabble.Services.Game
{
    using System.Collections.Generic;

    public class GameService : IGameService
    {
        private readonly IShuffleService shuffleService;
        private readonly ITilesProvider tilesProvider;

        public GameService(IShuffleService shuffleService, ITilesProvider tilesProvider)
        {
            this.shuffleService = shuffleService;
            this.tilesProvider = tilesProvider;
        }

        public GameState CreateGame(IEnumerable<string> userNames)
        {
            var shuffledTiles = this.shuffleService.Shuffle(this.tilesProvider.GetTiles());
            var tilesBag = new TilesBag(shuffledTiles);

            var shuffledUserNames = this.shuffleService.Shuffle(userNames);
            return new GameState(shuffledUserNames, tilesBag);
        }
    }
}

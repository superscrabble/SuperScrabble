namespace SuperScrabble.Services.Game
{
    using System.Linq;
    using System.Collections.Generic;

    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.ViewModels;

    public class GameService : IGameService
    {
        private readonly IShuffleService shuffleService;
        private readonly ITilesProvider tilesProvider;

        public GameService(IShuffleService shuffleService, ITilesProvider tilesProvider)
        {
            this.shuffleService = shuffleService;
            this.tilesProvider = tilesProvider;
        }

        public GameState CreateGame(IEnumerable<KeyValuePair<string, string>> connectionIdsByUserNames)
        {
            var shuffledTiles = this.shuffleService.Shuffle(this.tilesProvider.GetTiles());
            var tilesBag = new TilesBag(shuffledTiles);

            var shuffledUsers = this.shuffleService.Shuffle(connectionIdsByUserNames);
            return new GameState(shuffledUsers, tilesBag);
        }

        public void FillPlayerTiles(GameState gameState, string userName)
        {
            if (gameState.TilesCount == 0)
            {
                return;
            }

            Player player = gameState.GetPlayer(userName);

            int neededTilesCount = GameConstants.PlayerTilesCount - player.Tiles.Count;

            for (int i = 0; i < neededTilesCount; i++)
            {
                Tile tile = gameState.TilesBag.DrawTile();
                player.AddTile(tile);
            }
        }

        public PlayerGameStateViewModel MapFromGameState(GameState gameState, string userName)
        {
            var commonViewModel = new CommonGameStateViewModel
            {
                RemainingTilesCount = gameState.TilesCount,
                PointsByUserNames = gameState.Players.ToDictionary(p => p.UserName, p => p.Points),
            };

            var playerViewModel = new PlayerGameStateViewModel
            {
                Tiles = gameState.GetPlayer(userName).Tiles,
                CommonGameState = commonViewModel,
            };

            return playerViewModel;
        }
    }
}

namespace SuperScrabble.Services.Game
{
    using System.Linq;
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.ViewModels;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Services.Game.Models;

    public class GameService : IGameService
    {
        private readonly IShuffleService shuffleService;
        private readonly ITilesProvider tilesProvider;
        private readonly IBonusCellsProvider bonusCellsProvider;
        private readonly IGameplayConstantsProvider gameplayConstantsProvider;

        public GameService(
            IShuffleService shuffleService,
            ITilesProvider tilesProvider,
            IBonusCellsProvider bonusCellsProvider,
            IGameplayConstantsProvider gameplayConstantsProvider)
        {
            this.shuffleService = shuffleService;
            this.tilesProvider = tilesProvider;
            this.bonusCellsProvider = bonusCellsProvider;
            this.gameplayConstantsProvider = gameplayConstantsProvider;
        }

        public GameState CreateGame(IEnumerable<KeyValuePair<string, string>> connectionIdsByUserNames)
        {
            var shuffledTiles = this.shuffleService.Shuffle(this.tilesProvider.GetTiles());
            var tilesBag = new TilesBag(shuffledTiles);
            var positionsByBonusCells = this.bonusCellsProvider.GetPositionsByBonusCells();
            var board = new MyOldBoard(positionsByBonusCells);
            var shuffledUsers = this.shuffleService.Shuffle(connectionIdsByUserNames);
            return new GameState(shuffledUsers, tilesBag, board);
        }

        public void FillPlayerTiles(GameState gameState, string userName)
        {
            if (gameState.TilesCount == 0)
            {
                return;
            }

            Player player = gameState.GetPlayer(userName);

            int neededTilesCount = this.gameplayConstantsProvider.PlayerTilesCount - player.Tiles.Count;

            for (int i = 0; i < neededTilesCount; i++)
            {
                Tile tile = gameState.TilesBag.DrawTile();
                player.AddTile(tile);
            }
        }

        public PlayerGameStateViewModel MapFromGameState(GameState gameState, string userName)
        {
            var cellViewModels = new List<CellViewModel>();

            for (int row = 0; row < gameState.Board.Height; row++)
            {
                for (int col = 0; col < gameState.Board.Width; col++)
                {
                    var cell = gameState.Board[row, col];

                    var cellViewModel = new CellViewModel
                    {
                        Position = new Position(row, col),
                        Tile = cell.Tile,
                        Type = cell.Type,
                    };

                    cellViewModels.Add(cellViewModel);
                }
            }

            var boardViewModel = new BoardViewModel
            {
                Cells = cellViewModels,
                Width = gameState.Board.Width,
                Height = gameState.Board.Height
            };

            var commonViewModel = new CommonGameStateViewModel
            {
                RemainingTilesCount = gameState.TilesCount,
                PointsByUserNames = gameState.Players.ToDictionary(p => p.UserName, p => p.Points),
                Board = boardViewModel,
            };

            var playerViewModel = new PlayerGameStateViewModel
            {
                Tiles = gameState.GetPlayer(userName).Tiles,
                CommonGameState = commonViewModel,
            };

            return playerViewModel;
        }

        public WriteWordResult WriteWord(GameState gameState, WriteWordInputModel input, string authorUserName)
        {
            Player player = gameState.GetPlayer(authorUserName);
            WriteWordResult result = ValidatePlayerTiles(input, gameState, player);

            if (!result.IsSucceeded)
            {
                return result;
            }

            // TODO: Implement core functionality

            return result;
        }

        private static bool IsInputTilesCountValid(WriteWordInputModel input, Player player)
        {
            return !input.PositionsByTiles.Any() || input.PositionsByTiles.Count() > player.Tiles.Count;
        }

        private static WriteWordResult ValidatePlayerTiles(WriteWordInputModel input, GameState gameState, Player player)
        {
            var result = new WriteWordResult
            { 
                IsSucceeded = false
            };

            if (IsInputTilesCountValid(input, player))
            {
                result.ErrorsByCodes.Add("InvalidTilesCount", "");
                return result;
            }

            var playerTiles = player.Tiles.ToList();

            var uniqueRows = new HashSet<int>();
            var uniqueColumns = new HashSet<int>();

            foreach (var positionByTile in input.PositionsByTiles)
            {
                Tile tile = playerTiles.FirstOrDefault(pt => pt.Equals(positionByTile.Key));
                Position position = positionByTile.Value;

                uniqueRows.Add(position.Row);
                uniqueColumns.Add(position.Column);

                if (tile == null)
                {
                    result.ErrorsByCodes.Add("UnexistingPlayerTile", "");
                    return result;
                }

                if (!gameState.Board.IsPositionInside(position))
                {
                    result.ErrorsByCodes.Add("TilePositionOutsideBoardRange", "");
                    return result;
                }

                if (!gameState.Board.IsCellFree(position))
                {
                    result.ErrorsByCodes.Add("TilePositionAlreadyTaken", "");
                    return result;
                }

                playerTiles.Remove(tile);
            }

            if (uniqueRows.Count > 1 && uniqueColumns.Count > 1)
            {
                result.ErrorsByCodes.Add("TilesNotOnTheSameLine", "");
                return result;
            }

            bool hasRepeatingHorizontalPositions = uniqueRows.Count == 1
                && uniqueColumns.Count != input.PositionsByTiles.Count();

            bool hasRepeatingVerticalPositions = uniqueColumns.Count == 1
                && uniqueRows.Count != input.PositionsByTiles.Count();

            if (hasRepeatingHorizontalPositions || hasRepeatingVerticalPositions)
            {
                result.ErrorsByCodes.Add("InputTilesPositionsCollision", "");
                return result;
            }

            result.IsSucceeded = true;
            return result;
        }
    }
}

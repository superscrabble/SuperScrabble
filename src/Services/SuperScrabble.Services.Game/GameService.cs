namespace SuperScrabble.Services.Game
{
    using System.Linq;
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.ViewModels;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Data;

    public class GameService : IGameService
    {
        private readonly IShuffleService shuffleService;
        private readonly ITilesProvider tilesProvider;
        private readonly IBonusCellsProvider bonusCellsProvider;
        private readonly IGameplayConstantsProvider gameplayConstantsProvider;
        private readonly IWordsService wordsService;

        public GameService(
            IShuffleService shuffleService,
            ITilesProvider tilesProvider,
            IBonusCellsProvider bonusCellsProvider,
            IGameplayConstantsProvider gameplayConstantsProvider,
            IWordsService wordsService)
        {
            this.shuffleService = shuffleService;
            this.tilesProvider = tilesProvider;
            this.bonusCellsProvider = bonusCellsProvider;
            this.gameplayConstantsProvider = gameplayConstantsProvider;
            this.wordsService = wordsService;
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

            // determine whether letters are ordered horizontally or vertically
            // find left most or top most
            // start going left or up until an empty cell is found
            // 
            // TODO: Implement core functionality

            return result;
        }

        private static bool IsInputTilesCountValid(WriteWordInputModel input, Player player)
        {
            return !input.PositionsByTiles.Any() || input.PositionsByTiles.Count() > player.Tiles.Count;
        }

        private WriteWordResult ValidatePlayerTiles(WriteWordInputModel input, GameState gameState, Player player)
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

            bool areTilesOrderedVertically = uniqueRows.Count > uniqueColumns.Count;

            var sortedPositionsByTiles = areTilesOrderedVertically ? 
                input.PositionsByTiles.OrderBy(pt => pt.Value.Row) : 
                input.PositionsByTiles.OrderBy(pt => pt.Value.Column);

            IBoard board = gameState.Board;

            foreach (var positionByTile in sortedPositionsByTiles)
            {
                board[positionByTile.Value].Tile = positionByTile.Key;
            }

            if (areTilesOrderedVertically)
            {
                var wordBuilders = new List<WordBuilder>();

                var topMostPositionByTile = sortedPositionsByTiles.First();

                Position topMostPosition = topMostPositionByTile.Value;
                Position bottomMostPosition = sortedPositionsByTiles.Last().Value;

                var mainWordBuilder = new WordBuilder();
                mainWordBuilder.AppendUpwardExistingBoardTiles(board, topMostPosition);
                mainWordBuilder.AppendNewTiles(new[] { topMostPositionByTile });
                mainWordBuilder.AppendDownwardExistingBoardTiles(board, bottomMostPosition);
                wordBuilders.Add(mainWordBuilder);

                foreach (var positionByTile in sortedPositionsByTiles)
                {
                    var currentWordBuilder = new WordBuilder();
                    Position startingPosition = positionByTile.Value;
                    currentWordBuilder.AppendLeftwardExistingBoardTiles(board, startingPosition);
                    currentWordBuilder.AppendNewTiles(new[] { positionByTile });
                    currentWordBuilder.AppendRightwardExistingBoardTiles(board, startingPosition);
                    wordBuilders.Add(currentWordBuilder);
                }

                // If any of the newly formed words is invalid (db or others (new tiles don't touch any of the old tiles))
                // then revert board to initial state (before placing the new tiles)
            }
            else
            {
                Position leftMostPosition = sortedPositionsByTiles.First().Value;
            }

            result.IsSucceeded = true;
            return result;
        }
    }
}

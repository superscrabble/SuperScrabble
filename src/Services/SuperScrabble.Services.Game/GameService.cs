namespace SuperScrabble.Services.Game
{
    using System.Linq;
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.ViewModels;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Data;
    using System;

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
                if (gameState.TilesCount <= 0)
                {
                    break;
                }

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
                RestorePreviousBoardState(gameState.Board, input);
            }

            foreach (var positionByTile in input.PositionsByTiles)
            {
                gameState.GetPlayer(authorUserName).RemoveTile(positionByTile.Key);
            }
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

            bool areTilesAllignedVertically = uniqueRows.Count > uniqueColumns.Count;

            var sortedPositionsByTiles = areTilesAllignedVertically ?
                input.PositionsByTiles.OrderBy(pt => pt.Value.Row) :
                input.PositionsByTiles.OrderBy(pt => pt.Value.Column);


            Func<Position, Position> getNextPosition = areTilesAllignedVertically
                ? curr => new Position(curr.Row + 1, curr.Column)
                : curr => new Position(curr.Row, curr.Column + 1);

            bool goesThroughCenter = false;

            if (gameState.Board.IsEmpty())
            {
                goesThroughCenter = input.PositionsByTiles.Any(pbt => gameState.Board.IsPositionCenter(pbt.Value));

                if (!goesThroughCenter)
                {
                    result.ErrorsByCodes.Add("FirstWordMustBeOnTheBoardCenter", "");
                    return result;
                }
            }

            PlacePlayerTiles(gameState.Board, sortedPositionsByTiles);

            int passedPlayerTiles = 0;

            Position startingPosition = sortedPositionsByTiles.First().Value;
            var currentPosition = new Position(startingPosition.Row, startingPosition.Column);

            while (true)
            {
                if (!gameState.Board.IsPositionInside(currentPosition) || gameState.Board.IsCellFree(currentPosition))
                {
                    break;
                }

                if (sortedPositionsByTiles.Any(pbt => pbt.Value.Equals(currentPosition)))
                {
                    passedPlayerTiles++;
                }

                currentPosition = getNextPosition(currentPosition);
            }

            if (passedPlayerTiles != sortedPositionsByTiles.Count())
            {
                result.ErrorsByCodes.Add("GapsBetweenInputTilesNotAllowed", "");
                return result;
            }

            var (writeWordResult, words) = GetAllNewWords(gameState.Board, sortedPositionsByTiles, areTilesAllignedVertically, goesThroughCenter);

            if (!writeWordResult.IsSucceeded)
            {
                return writeWordResult;
            }

            result.IsSucceeded = true;
            return result;
        }

        private static void PlacePlayerTiles(IBoard board, IEnumerable<KeyValuePair<Tile, Position>> positionsByTiles)
        {
            foreach (var positionByTile in positionsByTiles)
            {
                board[positionByTile.Value].Tile = positionByTile.Key;
            }
        }

        private static void RestorePreviousBoardState(IBoard board, WriteWordInputModel input)
        {
            foreach (var positionByTile in input.PositionsByTiles)
            {
                board.FreeCell(positionByTile.Value);
            }
        }

        private static (WriteWordResult, IEnumerable<WordBuilder>) GetAllNewWords(
            IBoard board,
            IEnumerable<KeyValuePair<Tile, Position>> sortedPositionsByTiles,
            bool areTilesAllignedVertically,
            bool isThisTheFirstInput)
        {
            var result = new WriteWordResult() { IsSucceeded = false };

            Position startingPosition = sortedPositionsByTiles.First().Value;

            var wordBuilders = new List<WordBuilder>();
            var mainWordBuilder = new WordBuilder();

            if (areTilesAllignedVertically)
            {
                mainWordBuilder.AppendUpwardExistingBoardTiles(board, startingPosition);
                mainWordBuilder.AppendDownwardExistingBoardTiles(board, startingPosition);
            }
            else
            {
                mainWordBuilder.AppendLeftwardExistingBoardTiles(board, startingPosition);
                mainWordBuilder.AppendRightwardExistingBoardTiles(board, startingPosition);
            }

            wordBuilders.Add(mainWordBuilder);

            foreach (var positionByTile in sortedPositionsByTiles)
            {
                var secondaryWordBuilder = new WordBuilder();
                var currentStartingPosition = positionByTile.Value;

                if (areTilesAllignedVertically)
                {
                    secondaryWordBuilder.AppendLeftwardExistingBoardTiles(board, currentStartingPosition);
                    secondaryWordBuilder.AppendRightwardExistingBoardTiles(board, currentStartingPosition);
                }
                else
                {
                    secondaryWordBuilder.AppendUpwardExistingBoardTiles(board, currentStartingPosition);
                    secondaryWordBuilder.AppendDownwardExistingBoardTiles(board, currentStartingPosition);
                }

                if (secondaryWordBuilder.PositionsByTiles.Count <= 1)
                {
                    // Skip all new single letter words
                    continue;
                }

                wordBuilders.Add(secondaryWordBuilder);
            }

            if (isThisTheFirstInput)
            {
                result.IsSucceeded = true;
                return (result, wordBuilders);
            }

            if (wordBuilders.Count <= 1 && sortedPositionsByTiles.Count() == wordBuilders.First().PositionsByTiles.Count)
            {
                result.ErrorsByCodes.Add("NewTilesNotConnectedToTheOldOnes", "");
                return (result, null);
            }
            else
            {
                result.IsSucceeded = true;
                return (result, wordBuilders);
            }

            // SLOW BUT SECURE VALIDATION

            /*bool areNewTilesConnectedToTheOldOnes = false;

            foreach (WordBuilder wordBuilder in wordBuilders)
            {
                bool isWordDisconnected = wordBuilder
                    .PositionsByTiles.All(x => sortedPositionsByTiles.Any(p => p.Equals(x)));

                if (!isWordDisconnected)
                {
                    areNewTilesConnectedToTheOldOnes = true;
                    break;
                }
            }

            if (!areNewTilesConnectedToTheOldOnes)
            {
                result.ErrorsByCodes.Add("NewTilesNotConnectedToTheOldOnes", "");
            }
            else
            {
                result.IsSucceeded = true;
            }*/

            //return areNewTilesConnectedToTheOldOnes ? (result, wordBuilders) : (result, null); // TODO: Handle the error
        }
    }
}

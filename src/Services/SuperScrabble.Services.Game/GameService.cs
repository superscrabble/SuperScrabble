namespace SuperScrabble.Services.Game
{
    using System.Linq;
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.CustomExceptions.Game;

    using SuperScrabble.ViewModels;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.LanguageResources;

    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.Scoring;
    using SuperScrabble.Services.Game.Validation;
    using SuperScrabble.Services.Game.TilesProviders;
    using SuperScrabble.Services.Game.BonusCellsProviders;

    public class GameService : IGameService
    {
        private readonly IShuffleService shuffleService;
        private readonly ITilesProvider tilesProvider;
        private readonly IBonusCellsProvider bonusCellsProvider;
        private readonly IGameplayConstantsProvider gameplayConstantsProvider;
        private readonly IScoringService scoringService;
        private readonly IGameValidator gameValidator;

        public GameService(
            IShuffleService shuffleService,
            ITilesProvider tilesProvider,
            IBonusCellsProvider bonusCellsProvider,
            IGameplayConstantsProvider gameplayConstantsProvider,
            IScoringService scoringService,
            IGameValidator gameValidator)
        {
            this.shuffleService = shuffleService;
            this.tilesProvider = tilesProvider;
            this.bonusCellsProvider = bonusCellsProvider;
            this.gameplayConstantsProvider = gameplayConstantsProvider;
            this.scoringService = scoringService;
            this.gameValidator = gameValidator;
        }

        public GameState CreateGame(IEnumerable<KeyValuePair<string, string>> connectionIdsByUserNames)
        {
            var shuffledTiles = this.shuffleService.Shuffle(this.tilesProvider.GetTiles());
            var tilesBag = new TilesBag(shuffledTiles);

            var positionsByBonusCells = this.bonusCellsProvider.GetPositionsByBonusCells();
            var board = new MyOldBoard(positionsByBonusCells);

            var shuffledUsers = this.shuffleService.Shuffle(connectionIdsByUserNames);

            return new GameState(shuffledUsers, tilesBag, board, this.gameplayConstantsProvider);
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
                Height = gameState.Board.Height,
            };

            var commonViewModel = new CommonGameStateViewModel
            {
                RemainingTilesCount = gameState.TilesCount,

                Board = boardViewModel,

                PlayerOnTurnUserName = gameState.CurrentPlayer.UserName,

                IsTileExchangePossible = gameState.IsTileExchangePossible,

                IsGameOver = gameState.IsGameOver,

                PointsByUserNames = gameState.Players
                    .OrderByDescending(p => !p.HasLeftTheGame)
                    .ThenByDescending(p => p.Points)
                    .ToDictionary(p => p.UserName, p => p.Points),

                UserNamesOfPlayersWhoHaveLeftTheGame = gameState.Players.Where(p => p.HasLeftTheGame).Select(p => p.UserName),
            };

            var playerViewModel = new PlayerGameStateViewModel
            {
                Tiles = gameState.GetPlayer(userName).Tiles,
                CommonGameState = commonViewModel,
                MyUserName = userName,
            };

            return playerViewModel;
        }

        public GameOperationResult WriteWord(GameState gameState, WriteWordInputModel input, string authorUserName)
        {
            try
            {
                IBoard board = gameState.Board;
                Player author = gameState.GetPlayer(authorUserName);

                var inputTiles = input.PositionsByTiles.Select(x => x.Key);
                var inputPositions = input.PositionsByTiles.Select(x => x.Value);

                // Validate input model

                this.gameValidator.ValidateWhetherThePlayerIsOnTurn(gameState, authorUserName);

                this.gameValidator.ValidateInputTilesCount(
                    author.Tiles.Count, inputTiles.Count(), board.IsEmpty());

                this.gameValidator.ValidateWhetherPlayerHasSubmittedTilesWhichHeOwns(
                    author, inputTiles, isPlayerTryingToExchangeTiles: false);

                this.gameValidator.ValidateWhetherAllTilesAreInsideTheBoardRange(board, inputPositions);

                this.gameValidator.ValidateWhetherAllTilesPositionsAreFreeBoardCells(board, inputPositions);

                this.gameValidator.ValidateWhetherTilesAreOnTheSameLine(
                    inputPositions, out bool areTilesAllignedVertically);

                this.gameValidator.ValidateWhetherInputTilesHaveDuplicatePositions(inputPositions);

                this.gameValidator.ValidateWhetherFirstWordGoesThroughTheBoardCenter(
                    board, inputPositions, out bool goesThroughCenter);

                var sortedPositionsByTiles = areTilesAllignedVertically ?
                    input.PositionsByTiles.OrderBy(x => x.Value.Row) :
                    input.PositionsByTiles.OrderBy(x => x.Value.Column);

                PlacePlayerTiles(board, sortedPositionsByTiles);

                this.gameValidator.ValidateForGapsBetweenTheInputTiles(board, inputPositions, areTilesAllignedVertically);

                var wordBuilders = GetAllNewWords(
                    board, sortedPositionsByTiles, areTilesAllignedVertically, goesThroughCenter);

                this.gameValidator.ValidateWhetherTheWordsExist(wordBuilders.Select(wb => wb.ToString()));

                author.RemoveTiles(inputTiles);

                // Update score

                int newPoints = this.scoringService.CalculatePointsFromPlayerInput(input, gameState.Board, wordBuilders);

                if (input.PositionsByTiles.Count() == this.gameplayConstantsProvider.PlayerTilesCount)
                {
                    newPoints += this.gameplayConstantsProvider.BonusPointsForUsingAllTiles;
                }

                author.Points += newPoints;

                // Update game state

                if (author.Tiles.Count <= 0 && gameState.TilesCount <= 0)
                {
                    gameState.EndGame();
                }
                else
                {
                    gameState.NextPlayer();
                    gameState.ResetAllPlayersConsecutiveSkipsCount();
                }

                return new GameOperationResult { IsSucceeded = true };
            }
            catch (ValidationFailedException ex)
            {
                if (ex is ValidationFailedAfterInputTilesHaveBeenPlacedException)
                {
                    RestorePreviousBoardState(gameState.Board, input);
                }

                var result = new GameOperationResult { IsSucceeded = false };

                result.ErrorsByCodes.Add(ex.ErrorCode, ex.ErrorMessage);

                if (ex is UnexistingWordsException)
                {
                    var unexistingWordsException = ex as UnexistingWordsException;
                    result.UnexistingWords.AddRange(unexistingWordsException.UnexistingWords);
                }

                return result;
            }
        }

        public GameOperationResult ExchangeTiles(GameState gameState, ExchangeTilesInputModel input, string exchangerUserName)
        {
            try
            {
                Player exchanger = gameState.GetPlayer(exchangerUserName);

                this.gameValidator.ValidateWhetherThePlayerIsOnTurn(gameState, exchanger.UserName);

                if (!gameState.IsTileExchangePossible)
                {
                    throw new ValidationFailedException(
                        nameof(Resource.ImpossibleTileExchange), Resource.ImpossibleTileExchange);
                }

                this.gameValidator.ValidateWhetherPlayerHasSubmittedTilesWhichHeOwns(
                    exchanger, input.TilesToExchange, isPlayerTryingToExchangeTiles: true);

                var newTiles = new List<Tile>();

                for (int i = 0; i < input.TilesToExchange.Count(); i++)
                {
                    Tile newTile = gameState.TilesBag.DrawTile();
                    newTiles.Add(newTile);
                }

                gameState.TilesBag.AddTiles(input.TilesToExchange);

                exchanger.RemoveTiles(input.TilesToExchange);
                exchanger.AddTiles(newTiles);

                gameState.NextPlayer();
                gameState.ResetAllPlayersConsecutiveSkipsCount();

                return new GameOperationResult { IsSucceeded = true };
            }
            catch (ValidationFailedException ex)
            {
                var result = new GameOperationResult { IsSucceeded = false };
                result.ErrorsByCodes.Add(ex.ErrorCode, ex.ErrorMessage);
                return result;
            }
        }

        public GameOperationResult SkipTurn(GameState gameState, string skipperUserName)
        {
            try
            {
                this.gameValidator.ValidateWhetherThePlayerIsOnTurn(gameState, skipperUserName);

                Player player = gameState.GetPlayer(skipperUserName);
                player.ConsecutiveSkipsCount++;

                bool isGameOver = gameState.Players.All(
                    p => p.ConsecutiveSkipsCount >= this.gameplayConstantsProvider.MinSkipsCountForEachPlayerToEndTheGame);

                if (isGameOver)
                {
                    gameState.EndGame();
                }
                else
                {
                    gameState.NextPlayer();
                }

                return new GameOperationResult { IsSucceeded = true };
            }
            catch (ValidationFailedException ex)
            {
                var result = new GameOperationResult { IsSucceeded = false };
                result.ErrorsByCodes.Add(ex.ErrorCode, ex.ErrorMessage);
                return result;
            }
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

        private static IEnumerable<WordBuilder> GetAllNewWords(
            IBoard board,
            IEnumerable<KeyValuePair<Tile, Position>> sortedPositionsByTiles,
            bool areTilesAllignedVertically,
            bool isThisTheFirstInput)
        {
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

                wordBuilders.Add(secondaryWordBuilder);
            }

            wordBuilders = wordBuilders.Where(wb => wb.PositionsByTiles.Count > 1).ToList();

            if (isThisTheFirstInput)
            {
                return wordBuilders;
            }

            bool isNewTileDisonnectedFromTheOldTiles = (wordBuilders.Count <= 1
                && sortedPositionsByTiles.Count() == wordBuilders.FirstOrDefault()?.PositionsByTiles.Count)
                || wordBuilders.Count <= 0;

            if (isNewTileDisonnectedFromTheOldTiles)
            {
                throw new ValidationFailedAfterInputTilesHaveBeenPlacedException(
                    nameof(Resource.NewTilesNotConnectedToTheOldOnes), Resource.NewTilesNotConnectedToTheOldOnes);
            }

            return wordBuilders;
        }
    }
}

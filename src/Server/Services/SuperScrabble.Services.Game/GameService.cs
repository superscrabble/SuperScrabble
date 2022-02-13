namespace SuperScrabble.Services.Game
{
    using SuperScrabble.Common;
    using SuperScrabble.Common.Exceptions;
    using SuperScrabble.Common.Exceptions.Game;

    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.Models.Boards;
    using SuperScrabble.Services.Game.Scoring;
    using SuperScrabble.Services.Game.Validation;

    using SuperScrabble.WebApi.ViewModels.Game;

    public class GameService : IGameService
    {
        private readonly IScoringService scoringService;
        private readonly IGameValidator gameValidator;

        public GameService(
            IScoringService scoringService,
            IGameValidator gameValidator)
        {
            this.scoringService = scoringService;
            this.gameValidator = gameValidator;
        }

        public void FillPlayerTiles(GameState gameState, Player player)
        {
            if (gameState.TilesCount <= 0)
            {
                return;
            }

            int neededTilesCount = gameState.GameplayConstants.PlayerTilesCount - player.Tiles.Count;

            for (int i = 0; i < neededTilesCount; i++)
            {
                if (gameState.TilesCount <= 0)
                {
                    break;
                }

                Tile tile = gameState.Bag.DrawTile()!;
                player.AddTile(tile);
            }
        }

        public PlayerGameStateViewModel MapFromGameState(GameState gameState, string userName)
        {
            string message = $"Player with such username was not found.";

            Player player = gameState.GetPlayer(userName) ?? throw new ArgumentException(message);

            var commonGameStateViewModel = new CommonGameStateViewModel
            {
                Board = BoardAsViewModel(gameState.Board),
                Teams = gameState.Teams.Select(TeamAsViewModel),
                RemainingTilesCount = gameState.TilesCount,
                IsGameOver = gameState.IsGameOver,
                IsTileExchangePossible = gameState.IsTileExchangePossible,
                PlayerOnTurnUserName = gameState.CurrentTeam.CurrentPlayer.UserName,
                UserNamesOfPlayersWhoHaveLeftTheGame = gameState.GetUserNamesOfPlayersWhoHaveLeftTheGame(),
            };

            return new PlayerGameStateViewModel
            {
                MyUserName = player.UserName,
                Tiles = player.Tiles,
                CommonGameState = commonGameStateViewModel,
                TeammateTiles = gameState.GetTeam(userName)?
                    .Players.FirstOrDefault(pl => pl.UserName != userName)?.Tiles,
            };
        }

        private static TeamViewModel TeamAsViewModel(Team team)
        {
            return new TeamViewModel
            {
                Players = team.Players.Select(PlayerAsViewModel)
            };
        }

        private static PlayerViewModel PlayerAsViewModel(Player player)
        {
            return new PlayerViewModel
            {
                UserName = player.UserName,
                Points = player.Points,
            };
        }

        private static BoardViewModel BoardAsViewModel(IBoard board)
        {
            var cellViewModels = new List<CellViewModel>();

            for (int row = 0; row < board.Height; row++)
            {
                for (int col = 0; col < board.Width; col++)
                {
                    Cell cell = board[row, col];
                    cellViewModels.Add(CellAsViewModel(cell, row, col));
                }
            }

            return new BoardViewModel
            {
                Width = board.Width,
                Height = board.Height,
                Cells = cellViewModels,
            };
        }

        private static CellViewModel CellAsViewModel(Cell cell, int row, int column)
        {
            return new CellViewModel
            {
                Position = new Position(row, column),
                Tile = cell.Tile,
                Type = cell.Type,
            };
        }

        public GameOperationResult WriteWord(
            GameState gameState, WriteWordInputModel input, string authorUserName)
        {
            try
            {
                IBoard board = gameState.Board;

                var inputTiles = input.PositionsByTiles.Select(x => x.Key);
                var inputPositions = input.PositionsByTiles.Select(x => x.Value);

                this.gameValidator.IsPlayerOnTurn(gameState, authorUserName);

                Player author = gameState.GetPlayer(authorUserName)!;

                this.gameValidator.ValidateInputTilesCount(
                    author.Tiles.Count, inputTiles.Count(), board.IsEmpty());

                this.gameValidator.HasPlayerSubmittedTilesWhichHeOwns(
                    author, inputTiles, isPlayerTryingToExchangeTiles: false);

                this.gameValidator.AreAllTilesInsideTheBoardRange(board, inputPositions);

                this.gameValidator.AreAllTilesPositionsFreeBoardCells(board, inputPositions);

                this.gameValidator.AreTilesOnTheSameLine(
                    inputPositions, out bool areTilesAllignedVertically);

                this.gameValidator.DoInputTilesHaveDuplicatePositions(inputPositions);

                this.gameValidator.DoesFirstWordGoThroughTheBoardCenter(
                    board, inputPositions, out bool goesThroughCenter);

                var sortedPositionsByTiles = areTilesAllignedVertically ?
                    input.PositionsByTiles.OrderBy(x => x.Value.Row) :
                    input.PositionsByTiles.OrderBy(x => x.Value.Column);

                PlacePlayerTiles(board, sortedPositionsByTiles);

                this.gameValidator.ValidateForGapsBetweenTheInputTiles(
                    board, inputPositions, areTilesAllignedVertically);

                var wordBuilders = GetAllNewWords(
                    board, sortedPositionsByTiles, areTilesAllignedVertically, goesThroughCenter);

                this.gameValidator.ValidateWhetherWordsExist(wordBuilders.Select(wb => wb.ToString()));

                author.RemoveTiles(inputTiles);

                int newPoints = this.scoringService
                    .CalculatePointsFromPlayerInput(input, gameState.Board, wordBuilders);

                if (input.PositionsByTiles.Count() == gameState.GameplayConstants.PlayerTilesCount)
                {
                    newPoints += gameState.GameplayConstants.BonusPointsForUsingAllTiles;
                }

                author.Points += newPoints;

                if (author.Tiles.Count <= 0 && gameState.TilesCount <= 0)
                {
                    gameState.EndGame();
                }
                else
                {
                    gameState.CurrentTeam.NextPlayer();

                    if (gameState.CurrentTeam.IsTurnFinished)
                    {
                        gameState.CurrentTeam.ResetConsecutiveSkipsCount();
                        gameState.NextTeam();
                    }
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

                result.ErrorsByCodes.Add(ex.ErrorCode, ex.ErrorCode);

                if (ex is UnexistingWordsException unexistingWordsException)
                {
                    result.UnexistingWords.AddRange(unexistingWordsException.UnexistingWords);
                }

                return result;
            }
        }

        public GameOperationResult ExchangeTiles(
            GameState gameState, ExchangeTilesInputModel input, string exchangerUserName)
        {
            try
            {
                this.gameValidator.IsPlayerOnTurn(gameState, exchangerUserName);

                Player exchanger = gameState.GetPlayer(exchangerUserName)!;

                if (!gameState.IsTileExchangePossible)
                {
                    throw new ImpossibleTileExchangeException();
                }

                this.gameValidator.HasPlayerSubmittedTilesWhichHeOwns(
                    exchanger, input.TilesToExchange, isPlayerTryingToExchangeTiles: true);

                var newTiles = gameState.Bag.SwapTiles(input.TilesToExchange);

                exchanger.RemoveTiles(input.TilesToExchange);
                exchanger.AddTiles(newTiles);

                gameState.CurrentTeam.NextPlayer();

                if (gameState.CurrentTeam.IsTurnFinished)
                {
                    gameState.CurrentTeam.ResetConsecutiveSkipsCount();
                    gameState.NextTeam();
                }

                return new GameOperationResult { IsSucceeded = true };
            }
            catch (ValidationFailedException ex)
            {
                var result = new GameOperationResult { IsSucceeded = false };
                result.ErrorsByCodes.Add(ex.ErrorCode, ex.ErrorCode);
                return result;
            }
        }

        public GameOperationResult SkipTurn(GameState gameState, string skipperUserName)
        {
            try
            {
                this.gameValidator.IsPlayerOnTurn(gameState, skipperUserName);

                Player player = gameState.GetPlayer(skipperUserName)!;

                player.ConsecutiveSkipsCount++;

                bool isGameOver = gameState.Teams
                    .SelectMany(team => team.Players)
                    .All(p => p.ConsecutiveSkipsCount >= gameState
                        .GameplayConstants.MinSkipsCountForEachPlayerToEndTheGame);

                if (isGameOver)
                {
                    gameState.EndGame();
                }
                else
                {
                    gameState.CurrentTeam.NextPlayer();

                    if (gameState.CurrentTeam.IsTurnFinished)
                    {
                        gameState.NextTeam();
                    }
                }

                return new GameOperationResult { IsSucceeded = true };
            }
            catch (ValidationFailedException ex)
            {
                var result = new GameOperationResult { IsSucceeded = false };
                result.ErrorsByCodes.Add(ex.ErrorCode, ex.ErrorCode);
                return result;
            }
        }

        private static void PlacePlayerTiles(
            IBoard board, IEnumerable<KeyValuePair<Tile, Position>> positionsByTiles)
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
                throw new NewTilesNotConnectedToTheOldOnesException();
            }

            return wordBuilders;
        }
    }
}

﻿namespace SuperScrabble.Services.Game
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.CustomExceptions.Game;

    using SuperScrabble.ViewModels;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.LanguageResources;

    using SuperScrabble.Services.Data.Words;

    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.Scoring;
    using SuperScrabble.Services.Game.TilesProviders;
    using SuperScrabble.Services.Game.BonusCellsProviders;

    public class GameService : IGameService
    {
        private readonly IShuffleService shuffleService;
        private readonly ITilesProvider tilesProvider;
        private readonly IBonusCellsProvider bonusCellsProvider;
        private readonly IGameplayConstantsProvider gameplayConstantsProvider;
        private readonly IWordsService wordsService;
        private readonly IScoringService scoringService;

        public GameService(
            IShuffleService shuffleService,
            ITilesProvider tilesProvider,
            IBonusCellsProvider bonusCellsProvider,
            IGameplayConstantsProvider gameplayConstantsProvider,
            IWordsService wordsService,
            IScoringService scoringService)
        {
            this.shuffleService = shuffleService;
            this.tilesProvider = tilesProvider;
            this.bonusCellsProvider = bonusCellsProvider;
            this.gameplayConstantsProvider = gameplayConstantsProvider;
            this.wordsService = wordsService;
            this.scoringService = scoringService;
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
                Player author = gameState.GetPlayer(authorUserName);
                var wordBuilders = this.ValidateInputTilesAndExtractNewWords(input, gameState, author);

                foreach (var positionByTile in input.PositionsByTiles)
                {
                    author.RemoveTile(positionByTile.Key);
                }

                int newPoints = this.scoringService
                    .CalculatePointsFromPlayerInput(input, gameState.Board, wordBuilders);

                if (input.PositionsByTiles.Count() == this.gameplayConstantsProvider.PlayerTilesCount)
                {
                    newPoints += this.gameplayConstantsProvider.BonusPointsForUsingAllTiles;
                }

                author.Points += newPoints;

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

                this.ValidateExchangeTiles(gameState, input, exchanger);

                var newTiles = new List<Tile>();

                for (int i = 0; i < input.TilesToExchange.Count(); i++)
                {
                    Tile newTile = gameState.TilesBag.DrawTile();
                    newTiles.Add(newTile);
                }

                gameState.TilesBag.AddTiles(input.TilesToExchange);

                foreach (Tile tile in input.TilesToExchange)
                {
                    exchanger.RemoveTile(tile);
                }

                foreach (Tile tile in newTiles)
                {
                    exchanger.AddTile(tile);
                }

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
                ValidateWhetherThePlayerIsOnTurn(gameState, skipperUserName);

                Player player = gameState.GetPlayer(skipperUserName);
                player.ConsecutiveSkipsCount++;

                bool isGameOver = gameState.Players.All(p => p.ConsecutiveSkipsCount >= 2);

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

        private IEnumerable<WordBuilder> ValidateInputTilesAndExtractNewWords(WriteWordInputModel input, GameState gameState, Player player)
        {
            IBoard board = gameState.Board;

            ValidateWhetherThePlayerIsOnTurn(gameState, player.UserName);

            if (!IsInputTilesCountValid(input, player, board.IsEmpty()))
            {
                throw new ValidationFailedException(
                    nameof(Resource.InvalidInputTilesCount), Resource.InvalidInputTilesCount);
            }

            this.ValidateWhetherPlayerHasSubmittedTilesThatHeDoesOwn(player, input.PositionsByTiles.Select(x => x.Key));

            var uniqueRows = new HashSet<int>();
            var uniqueColumns = new HashSet<int>();

            foreach (var positionByTile in input.PositionsByTiles)
            {
                Position position = positionByTile.Value;

                uniqueRows.Add(position.Row);
                uniqueColumns.Add(position.Column);

                if (!board.IsPositionInside(position))
                {
                    throw new ValidationFailedException(
                        nameof(Resource.TilePositionOutsideBoardRange), Resource.TilePositionOutsideBoardRange);
                }

                if (!board.IsCellFree(position))
                {
                    throw new ValidationFailedException(
                        nameof(Resource.TilePositionAlreadyTaken), Resource.TilePositionAlreadyTaken);
                }
            }

            if (uniqueRows.Count > 1 && uniqueColumns.Count > 1)
            {
                throw new ValidationFailedException(
                    nameof(Resource.TilesNotOnTheSameLine), Resource.TilesNotOnTheSameLine);
            }

            bool hasRepeatingHorizontalPositions = uniqueRows.Count == 1
                && uniqueColumns.Count != input.PositionsByTiles.Count();

            bool hasRepeatingVerticalPositions = uniqueColumns.Count == 1
                && uniqueRows.Count != input.PositionsByTiles.Count();

            if (hasRepeatingHorizontalPositions || hasRepeatingVerticalPositions)
            {
                throw new ValidationFailedException(
                    nameof(Resource.InputTilesPositionsCollide), Resource.InputTilesPositionsCollide);
            }

            bool areTilesAllignedVertically = uniqueRows.Count > uniqueColumns.Count;

            var sortedPositionsByTiles = areTilesAllignedVertically ?
                input.PositionsByTiles.OrderBy(pt => pt.Value.Row) :
                input.PositionsByTiles.OrderBy(pt => pt.Value.Column);

            bool goesThroughCenter = false;

            if (board.IsEmpty())
            {
                goesThroughCenter = input.PositionsByTiles.Any(pbt => board.IsPositionCenter(pbt.Value));

                if (!goesThroughCenter)
                {
                    throw new ValidationFailedException(
                        nameof(Resource.FirstWordMustBeOnTheBoardCenter), Resource.FirstWordMustBeOnTheBoardCenter);
                }
            }

            PlacePlayerTiles(board, sortedPositionsByTiles);

            ValidateGapsBetweenTiles(board, sortedPositionsByTiles, areTilesAllignedVertically);

            var words = GetAllNewWords(
                board, sortedPositionsByTiles, areTilesAllignedVertically, goesThroughCenter);

            this.ValidateWhetherTheWordsExist(words);
            return words;
        }

        private void ValidateExchangeTiles(GameState gameState, ExchangeTilesInputModel input, Player exchanger)
        {
            ValidateWhetherThePlayerIsOnTurn(gameState, exchanger.UserName);

            if (!gameState.IsTileExchangePossible)
            {
                throw new ValidationFailedException(
                    nameof(Resource.ImpossibleTileExchange), Resource.ImpossibleTileExchange);
            }

            this.ValidateWhetherPlayerHasSubmittedTilesThatHeDoesOwn(
                exchanger, input.TilesToExchange, whenExchangingTiles: true);
        }

        private void ValidateWhetherPlayerHasSubmittedTilesThatHeDoesOwn(
            Player player, IEnumerable<Tile> tiles, bool whenExchangingTiles = false)
        {
            bool playerTileExchangeTilesSelector(Tile inputTile, Tile playerTile) => playerTile.Equals(inputTile);

            bool playerTileWriteWordSelector(Tile inputTile, Tile playerTile)
            {
                return playerTile.Equals(inputTile) || (inputTile?.Points == 0
                && playerTile.Letter == this.gameplayConstantsProvider.WildcardValue
                && playerTile.Points == 0);
            }

            Func<Tile, Tile, bool> playerTileSelector = whenExchangingTiles
                ? playerTileExchangeTilesSelector : playerTileWriteWordSelector;

            var wildcardOptions = this.tilesProvider.GetAllWildcardOptions();
            var playerTilesCopy = player.Tiles.ToList();

            foreach (Tile tile in tiles)
            {
                Tile playerTile = playerTilesCopy.FirstOrDefault(x => playerTileSelector(tile, x));

                if (playerTile == null)
                {
                    throw new ValidationFailedException(
                        nameof(Resource.UnexistingPlayerTile), Resource.UnexistingPlayerTile);
                }

                if (playerTile.Letter == gameplayConstantsProvider.WildcardValue && !whenExchangingTiles)
                {
                    if (wildcardOptions.FirstOrDefault(x => x.Letter == tile.Letter) == null)
                    {
                        throw new ValidationFailedException(
                            nameof(Resource.InvalidWildcardValue), Resource.InvalidWildcardValue);
                    }
                }

                playerTilesCopy.Remove(tile);
            }
        }

        private void ValidateWhetherTheWordsExist(IEnumerable<WordBuilder> wordsToValidate)
        {
            var unexistingWords = new List<WordBuilder>();

            foreach (WordBuilder wordToValidate in wordsToValidate)
            {
                if (!this.wordsService.IsWordValid(wordToValidate.ToString()))
                {
                    unexistingWords.Add(wordToValidate);
                }
            }

            if (unexistingWords.Count > 0)
            {
                throw new UnexistingWordsException(unexistingWords.Select(x => x.ToString()));
            }
        }

        private static bool IsInputTilesCountValid(WriteWordInputModel input, Player player, bool isBoardEmpty)
        {
            if (isBoardEmpty && input.PositionsByTiles.Count() < 2)
            {
                return false;
            }

            return input.PositionsByTiles.Any() && input.PositionsByTiles.Count() <= player.Tiles.Count;
        }

        private static void ValidateGapsBetweenTiles(
            IBoard board,
            IEnumerable<KeyValuePair<Tile, Position>> sortedPositionsByTiles,
            bool areTilesAllignedVertically)
        {
            Func<Position, Position> getNextPosition = areTilesAllignedVertically
                ? curr => new Position(curr.Row + 1, curr.Column)
                : curr => new Position(curr.Row, curr.Column + 1);

            int passedPlayerTiles = 0;
            Position startingPosition = sortedPositionsByTiles.First().Value;
            var currentPosition = new Position(startingPosition.Row, startingPosition.Column);

            while (true)
            {
                if (!board.IsPositionInside(currentPosition) || board.IsCellFree(currentPosition))
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
                throw new ValidationFailedAfterInputTilesHaveBeenPlacedException(
                    nameof(Resource.GapsBetweenInputTilesNotAllowed), Resource.GapsBetweenInputTilesNotAllowed);
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

        private static void ValidateWhetherThePlayerIsOnTurn(GameState gameState, string userName)
        {
            if (gameState.CurrentPlayer.UserName != userName)
            {
                throw new ValidationFailedException(
                    nameof(Resource.TheGivenPlayerIsNotOnTurn), Resource.TheGivenPlayerIsNotOnTurn);
            }
        }
    }
}

namespace SuperScrabble.Services.Game
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.ViewModels;
    using SuperScrabble.Services.Data;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.LanguageResources;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.CustomExceptions.Game;

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

                PointsByUserNames = gameState.Players.ToDictionary(
                    p => p.UserName, p => p.Points).OrderByDescending(pbu => pbu.Value),

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
            Player author = gameState.GetPlayer(authorUserName);
            Player currentPlayer = gameState.CurrentPlayer;

            if (author.UserName != currentPlayer.UserName)
            {
                var result = new WriteWordResult { IsSucceeded = false };

                result.ErrorsByCodes.Add(
                    nameof(Resource.TheGivenPlayerIsNotOnTurn), Resource.TheGivenPlayerIsNotOnTurn);

                return result;
            }

            try
            {
                var wordBuilders = ValidateInputTilesAndExtractWords(input, gameState, author);

                foreach (var positionByTile in input.PositionsByTiles)
                {
                    gameState.GetPlayer(authorUserName).RemoveTile(positionByTile.Key);
                }

                int newPoints = this.scoringService
                    .CalculatePointsFromPlayerInput(input, gameState.Board, wordBuilders);

                author.Points += newPoints;
                gameState.NextPlayer();

                return new WriteWordResult { IsSucceeded = true };
            }
            catch (ValidationFailedException ex)
            {
                RestorePreviousBoardState(gameState.Board, input);

                var result = new WriteWordResult { IsSucceeded = false };

                result.ErrorsByCodes.Add(ex.ErrorCode, ex.ErrorMessage);
                return result;
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

        private IEnumerable<WordBuilder> ValidateInputTilesAndExtractWords(WriteWordInputModel input, GameState gameState, Player player)
        {
            if (!IsInputTilesCountValid(input, player, gameState.Board.IsEmpty()))
            {
                throw new ValidationFailedException(
                    nameof(Resource.InvalidInputTilesCount), Resource.InvalidInputTilesCount);
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
                    throw new ValidationFailedException(
                        nameof(Resource.UnexistingPlayerTile), Resource.UnexistingPlayerTile);
                }

                if (!gameState.Board.IsPositionInside(position))
                {
                    throw new ValidationFailedException(
                        nameof(Resource.TilePositionOutsideBoardRange), Resource.TilePositionOutsideBoardRange);
                }

                if (!gameState.Board.IsCellFree(position))
                {
                    throw new ValidationFailedException(
                        nameof(Resource.TilePositionAlreadyTaken), Resource.TilePositionAlreadyTaken);
                }

                playerTiles.Remove(tile);
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

            if (gameState.Board.IsEmpty())
            {
                goesThroughCenter = input.PositionsByTiles.Any(pbt => gameState.Board.IsPositionCenter(pbt.Value));

                if (!goesThroughCenter)
                {
                    throw new ValidationFailedException(
                        nameof(Resource.FirstWordMustBeOnTheBoardCenter), Resource.FirstWordMustBeOnTheBoardCenter);
                }
            }

            PlacePlayerTiles(gameState.Board, sortedPositionsByTiles);

            ValidateGapsBetweenTiles(gameState.Board, sortedPositionsByTiles, areTilesAllignedVertically);

            var words = GetAllNewWords(
                gameState.Board, sortedPositionsByTiles, areTilesAllignedVertically, goesThroughCenter);

            List<WordBuilder> notExistingWords = new();

            foreach (var word in words)
            {
                if (!wordsService.IsWordValid(word.ToString().ToLower()))
                {
                    notExistingWords.Add(word);
                }
            }

            if (notExistingWords.Count > 0)
            {
                throw new ValidationFailedException(
                    nameof(Resource.WordDoesNotExist), Resource.WordDoesNotExist);
            }

            return words;
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
                throw new ValidationFailedException(
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

            if (wordBuilders.Count <= 1 && sortedPositionsByTiles.Count() == wordBuilders.First().PositionsByTiles.Count)
            {
                throw new ValidationFailedException(
                    nameof(Resource.NewTilesNotConnectedToTheOldOnes), Resource.NewTilesNotConnectedToTheOldOnes);
            }

            return wordBuilders;
        }
    }
}

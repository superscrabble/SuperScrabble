namespace SuperScrabble.Services.Game.Validation
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.CustomExceptions.Game;

    using SuperScrabble.Services.Data.Words;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.TilesProviders;

    public class GameValidator : IGameValidator
    {
        private readonly IGameplayConstantsProvider gameplayConstantsProvider;
        private readonly ITilesProvider tilesProvider;
        private readonly IWordsService wordsService;

        public GameValidator(
            IGameplayConstantsProvider gameplayConstantsProvider,
            ITilesProvider tilesProvider,
            IWordsService wordsService)
        {
            this.gameplayConstantsProvider = gameplayConstantsProvider;
            this.tilesProvider = tilesProvider;
            this.wordsService = wordsService;
        }

        public void IsPlayerOnTurn(GameState gameState, string userName)
        {
            if (gameState.CurrentPlayer.UserName != userName)
            {
                throw new PlayerNotOnTurnException();
            }
        }

        public void ValidateInputTilesCount(int playerTilesCount, int inputTilesCount, bool isBoardEmpty)
        {
            if (!IsInputTilesCountValid(playerTilesCount, inputTilesCount, isBoardEmpty))
            {
                throw new InvalidInputTilesCountException();
            }
        }

        public void HasPlayerSubmittedTilesWhichHeOwns(
            Player player, IEnumerable<Tile> submittedTiles, bool isPlayerTryingToExchangeTiles = false)
        {
            Func<Tile, Tile, bool> playerTileSelector = isPlayerTryingToExchangeTiles
                ? ExchangeTilesSelector
                : (submittedTile, playerTile) =>
                    WriteWordSelector(submittedTile, playerTile, this.gameplayConstantsProvider.WildcardValue);

            var wildcardOptions = this.tilesProvider.GetAllWildcardOptions();
            var playerTilesCopy = player.Tiles.ToList();

            foreach (Tile submittedTile in submittedTiles)
            {
                Tile actualTile = playerTilesCopy.FirstOrDefault(playerTile => playerTileSelector(submittedTile, playerTile));

                if (actualTile == null)
                {
                    throw new UnexistingPlayerTileException();
                }

                bool isTileToRemoveWildcard = actualTile.Letter == gameplayConstantsProvider.WildcardValue
                    && !isPlayerTryingToExchangeTiles;

                bool isTileToRemoveInvalidWildcardOption = wildcardOptions.FirstOrDefault(
                    option => option.Letter == submittedTile.Letter) == null;

                if (isTileToRemoveWildcard && isTileToRemoveInvalidWildcardOption)
                {
                    throw new InvalidWildcardValueException();
                }

                playerTilesCopy.Remove(submittedTile);
            }
        }

        public void AreAllTilesInsideTheBoardRange(IBoard board, IEnumerable<Position> inputTilesPositions)
        {
            foreach (Position position in inputTilesPositions)
            {
                if (!board.IsPositionInside(position))
                {
                    throw new TilePositionOutsideBoardRangeException();
                }
            }
        }

        public void AreAllTilesPositionsFreeBoardCells(IBoard board, IEnumerable<Position> inputTilesPositions)
        {
            foreach (Position position in inputTilesPositions)
            {
                if (!board.IsCellFree(position))
                {
                    throw new TilePositionAlreadyTakenException();
                }
            }
        }

        public void AreTilesOnTheSameLine(
            IEnumerable<Position> inputTilesPositions, out bool areTilesAllignedVertically)
        {
            var uniqueRows = inputTilesPositions.Select(pos => pos.Row).Distinct();
            var uniqueColumns = inputTilesPositions.Select(pos => pos.Column).Distinct();

            if (uniqueRows.Count() > 1 && uniqueColumns.Count() > 1)
            {
                throw new TilesNotOnTheSameLineException();
            }

            areTilesAllignedVertically = uniqueRows.Count() > uniqueColumns.Count();
        }

        public void DoesInputTilesHaveDuplicatePositions(IEnumerable<Position> inputTilesPositions)
        {
            var uniqueRows = inputTilesPositions.Select(pos => pos.Row).Distinct();
            var uniqueColumns = inputTilesPositions.Select(pos => pos.Column).Distinct();

            bool hasDuplicateHorizontalPositions = uniqueRows.Count() == 1
                && uniqueColumns.Count() != inputTilesPositions.Count();

            bool hasDuplicateVerticalPositions = uniqueColumns.Count() == 1
                && uniqueRows.Count() != inputTilesPositions.Count();

            if (hasDuplicateHorizontalPositions || hasDuplicateVerticalPositions)
            {
                throw new InputTilesPositionsCollideException();
            }
        }

        public void DoesFirstWordGoThroughTheBoardCenter(
            IBoard board, IEnumerable<Position> inputTilesPositions, out bool goesThroughCenter)
        {
            goesThroughCenter = false;

            if (board.IsEmpty())
            {
                goesThroughCenter = inputTilesPositions.Any(pos => board.IsPositionCenter(pos));

                if (!goesThroughCenter)
                {
                    throw new FirstWordMustBeOnTheBoardCenterException();
                }
            }
        }

        public void ValidateForGapsBetweenTheInputTiles(
            IBoard board, IEnumerable<Position> inputTilesPositions, bool areTilesAllignedVertically)
        {
            Func<Position, Position> getNextPosition = areTilesAllignedVertically
                ? curr => new Position(curr.Row + 1, curr.Column)
                : curr => new Position(curr.Row, curr.Column + 1);

            int passedPlayerTiles = 0;

            int sorter(Position pos) => areTilesAllignedVertically ? pos.Row : pos.Column;

            Position start = inputTilesPositions.OrderBy(sorter).First();
            Position current = new(start.Row, start.Column);

            while (true)
            {
                if (!board.IsPositionInside(current) || board.IsCellFree(current))
                {
                    break;
                }

                if (inputTilesPositions.Any(pos => pos.Equals(current)))
                {
                    passedPlayerTiles++;
                }

                current = getNextPosition(current);
            }

            if (passedPlayerTiles != inputTilesPositions.Count())
            {
                throw new GapsBetweenInputTilesNotAllowedException();
            }
        }

        public void ValidateWhetherWordsExist(IEnumerable<string> words)
        {
            var unexistingWords = new List<string>();

            foreach (string word in words)
            {
                if (!this.wordsService.IsWordValid(word))
                {
                    unexistingWords.Add(word);
                }
            }

            if (unexistingWords.Count > 0)
            {
                throw new UnexistingWordsException(unexistingWords);
            }
        }

        private static bool ExchangeTilesSelector(Tile submittedTile, Tile playerTile)
        {
            return playerTile.Equals(submittedTile);
        }

        private static bool WriteWordSelector(Tile submittedTile, Tile playerTile, char wildcardValue)
        {
            return playerTile.Equals(submittedTile)
                || (submittedTile?.Points == 0 && playerTile.Letter == wildcardValue && playerTile.Points == 0);
        }

        private static bool IsInputTilesCountValid(int playerTilesCount, int inputTilesCount, bool isBoardEmpty)
        {
            if (isBoardEmpty && inputTilesCount < 2)
            {
                return false;
            }

            return inputTilesCount > 0 && inputTilesCount <= playerTilesCount;
        }
    }
}

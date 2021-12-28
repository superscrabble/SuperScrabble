namespace SuperScrabble.Services.Game.Validation
{
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Models;

    public interface IGameValidator
    {
        void ValidateWhetherThePlayerIsOnTurn(GameState gameState, string userName);

        void ValidateInputTilesCount(int playerTilesCount, int inputTilesCount, bool isBoardEmpty);

        void ValidateWhetherPlayerHasSubmittedTilesWhichHeOwns(
            Player player, IEnumerable<Tile> submittedTiles, bool isPlayerTryingToExchangeTiles = false);

        void ValidateWhetherAllTilesAreInsideTheBoardRange(IBoard board, IEnumerable<Position> inputTilesPositions);

        void ValidateWhetherAllTilesPositionsAreFreeBoardCells(IBoard board, IEnumerable<Position> inputTilesPositions);

        /// <summary>
        /// Validates whether all input tiles are on the same vertical or horizontal line and throws an exception if not
        /// </summary>
        /// <param name="inputTilesPositions"></param>
        /// <returns>True if tiles are alligned vertically and False if horizontally</returns>
        void ValidateWhetherTilesAreOnTheSameLine(
            IEnumerable<Position> inputTilesPositions, out bool areTilesAllignedVertically);

        void ValidateWhetherInputTilesHaveDuplicatePositions(IEnumerable<Position> inputTilesPositions);

        void ValidateWhetherFirstWordGoesThroughTheBoardCenter(
            IBoard board, IEnumerable<Position> inputTilesPositions, out bool goesThroughCenter);

        void ValidateForGapsBetweenTheInputTiles(
            IBoard board, IEnumerable<Position> inputTilesPositions, bool areTilesAllignedVertically);

        void ValidateWhetherTheWordsExist(IEnumerable<string> words);
    }
}

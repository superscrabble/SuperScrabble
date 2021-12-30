namespace SuperScrabble.Services.Game.Validation
{
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Models;

    public interface IGameValidator
    {
        void IsPlayerOnTurn(GameState gameState, string userName);

        void ValidateInputTilesCount(int playerTilesCount, int inputTilesCount, bool isBoardEmpty);

        void HasPlayerSubmittedTilesWhichHeOwns(
            Player player, IEnumerable<Tile> submittedTiles, bool isPlayerTryingToExchangeTiles = false);

        void AreAllTilesInsideTheBoardRange(IBoard board, IEnumerable<Position> inputTilesPositions);

        void AreAllTilesPositionsFreeBoardCells(IBoard board, IEnumerable<Position> inputTilesPositions);

        void AreTilesOnTheSameLine(
            IEnumerable<Position> inputTilesPositions, out bool areTilesAllignedVertically);

        void DoesInputTilesHaveDuplicatePositions(IEnumerable<Position> inputTilesPositions);

        void DoesFirstWordGoThroughTheBoardCenter(
            IBoard board, IEnumerable<Position> inputTilesPositions, out bool goesThroughCenter);

        void ValidateForGapsBetweenTheInputTiles(
            IBoard board, IEnumerable<Position> inputTilesPositions, bool areTilesAllignedVertically);

        void ValidateWhetherWordsExist(IEnumerable<string> words);
    }
}

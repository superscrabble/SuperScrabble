namespace SuperScrabble.Services.Game.Validation
{
    using SuperScrabble.Common;
    using SuperScrabble.Common.Exceptions.Game;

    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.Models.Boards;

    public interface IGameValidator
    {
        /// <summary>
        /// Checks whether the player with the given userName is present inside the given game
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="playerUserName"></param>
        void IsPlayerInsideGame(GameState gameState, string playerUserName, out Player player);

        /// <summary>
        /// Checks whether the player with the given userName is on turn.
        /// Otherwise an exception is thrown
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="userName"></param>
        /// <exception cref="PlayerNotOnTurnException"></exception>
        void IsPlayerOnTurn(GameState gameState, string userName);

        /// <summary>
        /// Checks whether the number of tiles which the player has submitted is valid.
        /// Otherwise an exception is thrown
        /// </summary>
        /// <param name="playerTilesCount"></param>
        /// <param name="inputTilesCount"></param>
        /// <param name="isBoardEmpty"></param>
        /// <exception cref="InvalidInputTilesCountException"></exception>
        void ValidateInputTilesCount(int playerTilesCount, int inputTilesCount, bool isBoardEmpty);

        /// <summary>
        /// Checks whether the given player has submitted tiles which he actually owns.
        /// Otherwise an exception is thrown
        /// </summary>
        /// <param name="player"></param>
        /// <param name="submittedTiles"></param>
        /// <param name="isPlayerTryingToExchangeTiles"></param>
        /// <exception cref="UnexistingPlayerTilesException"></exception>
        /// <exception cref="InvalidWildcardValueException"></exception>
        void HasPlayerSubmittedTilesWhichHeOwns(
            Player player, IEnumerable<Tile> submittedTiles, bool isPlayerTryingToExchangeTiles = false);

        /// <summary>
        /// Checks whether all submitted tiles are inside the given board range.
        /// Otherwise an exception is thrown
        /// </summary>
        /// <param name="board"></param>
        /// <param name="inputTilesPositions"></param>
        /// <exception cref="TilePositionOutsideBoardRangeException"></exception>
        void AreAllTilesInsideTheBoardRange(IBoard board, IEnumerable<Position> inputTilesPositions);

        /// <summary>
        /// Checks whether none of the submitted tile positions are already taken.
        /// Otherwise an exception is thrown
        /// </summary>
        /// <param name="board"></param>
        /// <param name="inputTilesPositions"></param>
        /// <exception cref="TilePositionAlreadyTakenException"></exception>
        void AreAllTilesPositionsFreeBoardCells(IBoard board, IEnumerable<Position> inputTilesPositions);

        /// <summary>
        /// Checks whether all of the submitted tiles are placed on the same horizontal or vertical line.
        /// Otherwise an exception is thrown
        /// </summary>
        /// <param name="inputTilesPositions"></param>
        /// <param name="areTilesAllignedVertically"></param>
        /// <exception cref="TilesNotOnTheSameLineException"></exception>
        void AreTilesOnTheSameLine(
            IEnumerable<Position> inputTilesPositions, out bool areTilesAllignedVertically);

        /// <summary>
        /// Checks whether all submitted tiles are placed on unique positions.
        /// Otherwise an exception is thrown
        /// </summary>
        /// <param name="inputTilesPositions"></param>
        /// <exception cref="InputTilesPositionsCollideException"></exception>
        void DoInputTilesHaveDuplicatePositions(IEnumerable<Position> inputTilesPositions);

        /// <summary>
        /// Checks whether this is the first word on the board and whether the word goes through the center of the board.
        /// Otherwise an exception is thrown
        /// </summary>
        /// <param name="board"></param>
        /// <param name="inputTilesPositions"></param>
        /// <param name="goesThroughCenter"></param>
        /// <exception cref="FirstWordMustGoThroughTheBoardCenterException"></exception>
        void DoesFirstWordGoThroughTheBoardCenter(
            IBoard board, IEnumerable<Position> inputTilesPositions, out bool goesThroughCenter);

        /// <summary>
        /// Checks whether there are no wholes between the tiles placed on the board.
        /// Otherwise an exception is thrown
        /// </summary>
        /// <param name="board"></param>
        /// <param name="inputTilesPositions"></param>
        /// <param name="areTilesAllignedVertically"></param>
        /// <exception cref="GapsBetweenInputTilesNotAllowedException"></exception>
        void ValidateForGapsBetweenTheInputTiles(
            IBoard board, IEnumerable<Position> inputTilesPositions, bool areTilesAllignedVertically);

        /// <summary>
        /// Checks whether the newly written words are actually existing words.
        /// Otherwise an exception is thrown
        /// </summary>
        /// <param name="words"></param>
        /// <exception cref="UnexistingWordsException"></exception>
        void ValidateWhetherWordsExist(IEnumerable<string> words);
    }
}

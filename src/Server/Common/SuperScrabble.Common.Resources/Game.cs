namespace SuperScrabble.Common.Resources
{
    public static class Game
    {
        public static class GameplayErrorCodes
        {
            public const string PlayerIsNotOnTurn = "PlayerIsNotOnTurn";
            public const string InvalidInputTilesCount = "InvalidInputTilesCount";
            public const string UnexistingPlayerTiles = "UnexistingPlayerTiles";
            public const string InvalidWildcardValue = "InvalidWildcardValue";
            public const string TilePositionOutsideBoardRange = "TilePositionOutsideBoardRange";
            public const string TilePositionAlreadyTaken = "TilePositionAlreadyTaken";
            public const string TilesNotOnTheSameLine = "TilesNotOnTheSameLine";
            public const string InputTilesPositionsCollide = "InputTilesPositionsCollide";
            public const string FirstWordMustGoThroughTheBoardCenter = "FirstWordMustGoThroughTheBoardCenter";
            public const string GapsBetweenInputTilesNotAllowed = "GapsBetweenInputTilesNotAllowed";
            public const string WordsDoNotExist = "UnexistingWords";
        }
    }
}

namespace SuperScrabble.Services.Game.Models
{
    using System;

    public enum CellType
    {
        Basic = 0,
        Center = 1,
        DoubleLetter = 2,
        TripleLetter = 3,
        DoubleWord = 4,
        TripleWord = 5,
    }

    public static class CellTypeExtensions
    {
        public static bool IsWordBonus(this CellType cellType) =>
            cellType == CellType.DoubleWord || cellType == CellType.TripleWord;

        public static bool IsLetterBonus(this CellType cellType) =>
            cellType == CellType.DoubleLetter || cellType == CellType.TripleLetter;

        public static int GetPointsMultiplier(this CellType cellType)
        {
            if (cellType == CellType.Basic || cellType == CellType.Center)
            {
                return 1;
            }

            if (cellType == CellType.DoubleLetter || cellType == CellType.DoubleWord)
            {
                return 2;
            }

            if (cellType == CellType.TripleLetter || cellType == CellType.TripleWord)
            {
                return 3;
            }

            throw new NotSupportedException($"Unsupported {nameof(CellType)} enum value.");
        }
    }
}

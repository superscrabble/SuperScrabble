namespace SuperScrabble.Services.Game.Common.Enums
{
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
        private static readonly CellType[] wordBonuses = new[]
        {
            CellType.DoubleWord,
            CellType.TripleWord,
            CellType.Center,
        };

        private static readonly CellType[] letterBonuses = new[]
        {
            CellType.DoubleLetter,
            CellType.TripleLetter,
        };

        private static readonly Dictionary<CellType, int> pointsMultipliersByCellTypes = new()
        {
            [CellType.Basic] = 1,
            [CellType.DoubleLetter] = 2,
            [CellType.DoubleWord] = 2,
            [CellType.Center] = 2,
            [CellType.TripleLetter] = 3,
            [CellType.TripleWord] = 3,
        };

        public static bool IsWordBonus(this CellType cellType)
        {
            return wordBonuses.Contains(cellType);
        }

        public static bool IsLetterBonus(this CellType cellType)
        {
            return letterBonuses.Contains(cellType);
        }

        public static int GetPointsMultiplier(this CellType cellType)
        {
            if (!pointsMultipliersByCellTypes.ContainsKey(cellType))
            {
                throw new NotSupportedException($"Unsupported {nameof(CellType)} enum value.");
            }

            return pointsMultipliersByCellTypes[cellType];
        }
    }
}

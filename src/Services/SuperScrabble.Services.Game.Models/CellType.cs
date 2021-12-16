namespace SuperScrabble.Services.Game.Models
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
        public static bool IsWordBonus(this CellType cellType) =>
            cellType == CellType.DoubleWord || cellType == CellType.TripleWord;

        public static bool IsLetterBonus(this CellType cellType) =>
            cellType == CellType.DoubleLetter || cellType == CellType.TripleLetter;
    }
}

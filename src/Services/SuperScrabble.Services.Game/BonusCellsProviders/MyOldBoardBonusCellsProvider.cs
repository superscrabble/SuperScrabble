namespace SuperScrabble.Services.Game.BonusCellsProviders
{
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Models;

    public class MyOldBoardBonusCellsProvider : IBonusCellsProvider
    {
        public Dictionary<CellType, List<Position>> GetPositionsByBonusCells()
        {
            return new()
            {
                [CellType.Center] = new() { new(7, 7) },

                [CellType.TripleWord] = new()
                {
                    new(0, 0),
                    new(0, 7),
                    new(0, 14),
                    new(7, 0),
                    new(7, 14),
                    new(14, 0),
                    new(14, 7),
                    new(14, 14),
                },

                [CellType.TripleLetter] = new()
                {
                    new(1, 5),
                    new(1, 9),
                    new(5, 1),
                    new(5, 5),
                    new(5, 9),
                    new(5, 13),
                    new(9, 1),
                    new(9, 5),
                    new(9, 9),
                    new(9, 13),
                    new(13, 5),
                    new(13, 9),
                },

                [CellType.DoubleWord] = new()
                {
                    new(1, 1),
                    new(1, 13),
                    new(2, 2),
                    new(2, 12),
                    new(3, 3),
                    new(3, 11),
                    new(4, 4),
                    new(4, 10),
                    new(10, 4),
                    new(10, 10),
                    new(11, 3),
                    new(11, 11),
                    new(12, 2),
                    new(12, 12),
                    new(13, 1),
                    new(13, 13),
                },

                [CellType.DoubleLetter] = new()
                {
                    new(0, 3),
                    new(0, 11),
                    new(2, 6),
                    new(2, 8),
                    new(3, 0),
                    new(3, 7),
                    new(3, 14),
                    new(6, 2),
                    new(6, 6),
                    new(6, 8),
                    new(6, 12),
                    new(7, 3),
                    new(7, 11),
                    new(8, 2),
                    new(8, 6),
                    new(8, 8),
                    new(8, 12),
                    new(11, 0),
                    new(11, 7),
                    new(11, 14),
                    new(12, 6),
                    new(12, 8),
                    new(14, 3),
                    new(14, 11),
                },
            };
        }
    }
}

using System;
using System.Collections.Generic;

namespace SuperScrabble.Services.Game
{
    public class TilesBag
    {
        private class TileInfo
        {
            public TileInfo(int points, int count)
            {
                Points = points;
                Count = count;
            }

            public int Points { get; set; }
            public int Count { get; set; }
        }

        //TODO: store tiles as dictionary => {letter: count}
        private IList<Tile> tiles;

        public int TilesCount;

        private static readonly Dictionary<char, TileInfo> letters = new()
        {
            ['А'] = new(1, 9),
            ['Б'] = new(2, 3),
            ['В'] = new(2, 4),
            ['Г'] = new(3, 3),
            ['Д'] = new(2, 4),
            ['Е'] = new(1, 8),
            ['Ж'] = new(4, 2),
            ['З'] = new(4, 2),
            ['И'] = new(1, 8),
            ['Й'] = new(5, 1),
            ['К'] = new(2, 3),
            ['Л'] = new(2, 3),
            ['М'] = new(2, 4),
            ['Н'] = new(1, 4),
            ['О'] = new(1, 9),
            ['П'] = new(1, 4),
            ['Р'] = new(1, 4),
            ['С'] = new(1, 4),
            ['Т'] = new(1, 5),
            ['У'] = new(5, 3),
            ['Ф'] = new(10, 1),
            ['Х'] = new(5, 1),
            ['Ц'] = new(8, 1),
            ['Ч'] = new(5, 2),
            ['Ш'] = new(8, 1),
            ['Щ'] = new(10, 1),
            ['Ъ'] = new(3, 2),
            ['ь'] = new(10, 1),
            ['Ю'] = new(8, 1),
            ['Я'] = new(5, 2),
            ['\0'] = new(0, 2) //Wild Card
        };

        public TilesBag()
        {
            foreach (var letter in letters)
            {
                tiles.Add(new Tile { Letter = letter.Key, Points = letter.Value.Points });
            }
        }

        public Tile getTile()
        {
            throw new NotImplementedException();
        }
    }
}
using SuperScrabble.Services.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.Services.Game.TilesProviders
{
    public class FakeTilesProvider : ITilesProvider
    {
        public IEnumerable<KeyValuePair<char, TileInfo>> GetTiles()
        {
            return new Dictionary<char, TileInfo>()
            {
                ['А'] = new(1, 1),
                ['Б'] = new(2, 1),
                ['В'] = new(2, 1),
                ['Г'] = new(3, 1),
                ['Д'] = new(2, 1),
                ['Е'] = new(1, 1),
                ['Ж'] = new(4, 1),
                ['З'] = new(4, 1),
                ['И'] = new(1, 1),
                ['Й'] = new(5, 1),
                ['К'] = new(2, 2),
                ['Л'] = new(2, 2),
                ['М'] = new(2, 2),
                ['Н'] = new(1, 2),
                ['О'] = new(1, 2),
            };
        }
    }
}

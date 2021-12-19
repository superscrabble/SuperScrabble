namespace SuperScrabble.Services.Game.TilesProviders
{
    using System.Collections.Generic;

    using SuperScrabble.Services.Game.Models;

    public class MyOldBoardTilesProvider : ITilesProvider
    {
        private readonly IGameplayConstantsProvider gameplayConstantsProvider;

        public MyOldBoardTilesProvider(IGameplayConstantsProvider gameplayConstantsProvider)
        {
            this.gameplayConstantsProvider = gameplayConstantsProvider;
        }

        public IEnumerable<KeyValuePair<char, TileInfo>> GetTiles()
        {
            return new Dictionary<char, TileInfo>()
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
                ['Ь'] = new(10, 1),
                ['Ю'] = new(8, 1),
                ['Я'] = new(5, 2),
                [this.gameplayConstantsProvider.WildcardValue] = new(0, 2),
            };
        }
    }
}

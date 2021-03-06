namespace SuperScrabble.Services.Game.BonusCellsProviders
{
    using System.Collections.Generic;

    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Models;

    public interface IBonusCellsProvider
    {
        Dictionary<CellType, List<Position>> GetPositionsByBonusCells();
    }
}

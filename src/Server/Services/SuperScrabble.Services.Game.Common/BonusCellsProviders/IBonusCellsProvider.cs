namespace SuperScrabble.Services.Game.Common.BonusCellsProviders
{
    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Common.Enums;

    public interface IBonusCellsProvider
    {
        Dictionary<CellType, List<Position>> GetPositionsByBonusCells();
    }
}

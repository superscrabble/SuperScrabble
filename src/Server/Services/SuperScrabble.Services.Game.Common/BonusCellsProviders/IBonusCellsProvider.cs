namespace SuperScrabble.Services.Game.Common.BonusCellsProviders
{
    using SuperScrabble.Common;

    public interface IBonusCellsProvider
    {
        Dictionary<CellType, List<Position>> GetPositionsByBonusCells();
    }
}

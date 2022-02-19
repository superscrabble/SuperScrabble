using SuperScrabble.Services.Game.Common.BonusCellsProviders;

namespace SuperScrabble.Services.Game.Models.Boards;

public class StandardBoard : Board
{
    private const int StandardBoardWidth = 15;
    private const int StandardBoardHeight = 15;

    public StandardBoard(IBonusCellsProvider bonusCellsProvider)
        : base(StandardBoardWidth, StandardBoardHeight, bonusCellsProvider)
    {
    }
}

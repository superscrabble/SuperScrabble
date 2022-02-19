﻿using SuperScrabble.Services.Game.Common.BonusCellsProviders;

namespace SuperScrabble.Services.Game.Models.Boards;

public class GiantBoard : Board
{
    private const int GiantBoardWidth = 21;
    private const int GiantBoardHeight = 21;

    public GiantBoard(IBonusCellsProvider bonusCellsProvider)
        : base(GiantBoardWidth, GiantBoardHeight, bonusCellsProvider)
    {
    }
}

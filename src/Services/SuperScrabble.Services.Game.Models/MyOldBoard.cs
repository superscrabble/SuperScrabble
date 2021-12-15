namespace SuperScrabble.Services.Game.Models
{
    using System.Collections.Generic;

    using SuperScrabble.Common;

    public class MyOldBoard : Board
    {
        private const int MyOldBoardWidth = 15;
        private const int MyOldBoardHeight = 15;

        public MyOldBoard(Dictionary<CellType, List<Position>> positionsByBonusCellTypes)
            : base(MyOldBoardWidth, MyOldBoardHeight, positionsByBonusCellTypes)
        {
        }
    }
}

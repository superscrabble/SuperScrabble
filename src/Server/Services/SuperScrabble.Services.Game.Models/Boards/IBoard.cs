﻿namespace SuperScrabble.Services.Game.Models.Boards
{
    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Common;

    public interface IBoard
    {
        int Height { get; }

        int Width { get; }

        Cell this[int row, int column] { get; set; }

        Cell this[Position position] { get; set; }

        void FreeCell(Position position);

        bool IsCellFree(Position position);

        bool IsPositionInside(Position position);

        bool IsEmpty();

        bool IsPositionCenter(Position position);
    }
}
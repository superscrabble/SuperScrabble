﻿using SuperScrabble.Common;
using SuperScrabble.Services.Game.Common;
using SuperScrabble.Services.Game.Common.BonusCellsProviders;
using SuperScrabble.Services.Game.Common.Enums;

namespace SuperScrabble.Services.Game.Models.Boards;

public abstract class Board : IBoard
{
    private readonly Cell[,] _cells;
    private readonly Dictionary<CellType, List<Position>> positionsByBonusCellTypes;

    protected Board(int width, int height, IBonusCellsProvider bonusCellsProvider)
    {
        _cells = new Cell[width, height];
        positionsByBonusCellTypes = bonusCellsProvider.GetPositionsByBonusCells();

        InitializeAllCellsAsBasic();
        InitializeBonusCells();
    }

    private Position Center => new(Height / 2, Width / 2);

    public int Height => _cells.GetLength(0);

    public int Width => _cells.GetLength(1);

    public Cell this[Position position]
    {
        get => this[position.Row, position.Column];

        set => this[position.Row, position.Column] = value;
    }

    public Cell this[int row, int column]
    {
        get => _cells[row, column];

        set => _cells[row, column] = value;
    }

    public bool IsCellFree(Position position)
    {
        Cell cell = this[position.Row, position.Column];
        return cell.Tile == null;
    }

    public bool IsPositionInside(Position position)
    {
        return position.Row >= 0 && position.Column >= 0
            && position.Row < Height && position.Column < Width;
    }

    /// <summary>
    /// Sets the tile on the cell with the given position to null
    /// </summary>
    /// <param name="position"></param>
    public void FreeCell(Position position)
    {
        this[position].Tile = null;
    }

    /// <summary>
    /// </summary>
    /// <param name="position"></param>
    /// <returns>True if the given position is the center of the board, otherwise False</returns>
    public virtual bool IsPositionCenter(Position position)
    {
        return Center.Equals(position);
    }

    public virtual bool IsEmpty()
    {
        return this[Height / 2, Width / 2].Tile == null;
    }

    private void InitializeAllCellsAsBasic()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                this[row, col] = new Cell(CellType.Basic);
            }
        }
    }

    private void InitializeBonusCells()
    {
        foreach (var positionsByCellType in positionsByBonusCellTypes)
        {
            foreach (Position position in positionsByCellType.Value)
            {
                this[position.Row, position.Column] = new Cell(positionsByCellType.Key);
            }
        }
    }
}

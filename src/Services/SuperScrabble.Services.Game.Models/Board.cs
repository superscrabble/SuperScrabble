namespace SuperScrabble.Services.Game.Models
{
    using System.Collections.Generic;

    using SuperScrabble.Common;

    public abstract class Board : IBoard
    {
        private readonly Cell[,] cells;
        private readonly Dictionary<CellType, List<Position>> positionsByBonusCellTypes;

        protected Board(
            int width,
            int height,
            Dictionary<CellType, List<Position>> positionsByBonusCellTypes)
        {
            this.cells = new Cell[width, height];
            this.positionsByBonusCellTypes = positionsByBonusCellTypes;

            this.InitializeAllCellsAsBasic();
            this.InitializeBonusCells();
        }

        public int Height => this.cells.GetLength(0);

        public int Width => this.cells.GetLength(1);

        public Cell this[Position position]
        { 
            get => this[position.Row, position.Column];

            set => this[position.Row, position.Column] = value;
        }
        
        public Cell this[int row, int column]
        {
            get => this.cells[row, column];

            set => this.cells[row, column] = value;
        }

        private void InitializeAllCellsAsBasic()
        {
            for (int row = 0; row < this.Height; row++)
            {
                for (int col = 0; col < this.Width; col++)
                {
                    this[row, col] = new Cell(CellType.Basic);
                }
            }
        }

        private void InitializeBonusCells()
        {
            foreach (var positionsByCellType in this.positionsByBonusCellTypes)
            {
                foreach (Position position in positionsByCellType.Value)
                {
                    this[position.Row, position.Column] = new Cell(positionsByCellType.Key);
                }
            }
        }

        public bool IsCellFree(Position position)
        {
            Cell cell = this[position.Row, position.Column];
            return cell.Tile == null;
        }

        public bool IsPositionInside(Position position)
        {
            return position.Row >= 0 && position.Column >= 0 
                && position.Row < this.Height && position.Column < this.Width;
        }

        public void FreeCell(Position position)
        {
            this[position].Tile = null;
        }
    }
}

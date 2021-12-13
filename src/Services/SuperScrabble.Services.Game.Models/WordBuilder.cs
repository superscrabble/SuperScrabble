namespace SuperScrabble.Services.Game.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using SuperScrabble.Common;

    public class WordBuilder
    {
        private readonly LinkedList<KeyValuePair<Tile, Position>> positionsByTiles = new();

        public IReadOnlyCollection<KeyValuePair<Tile, Position>> PositionsByTiles =>
            this.positionsByTiles.ToList().AsReadOnly();

        public void AppendLeftwardExistingBoardTiles(IBoard board, Position startingPosition)
        {
            this.AppendExistingBoardTiles(
                board, startingPosition, x => new Position(x.Row, x.Column - 1), toFront: true);
        }

        public void AppendRightwardExistingBoardTiles(IBoard board, Position startingPosition)
        {
            this.AppendExistingBoardTiles(
                board, startingPosition, x => new Position(x.Row, x.Column + 1), toFront: false);
        }

        public void AppendUpwardExistingBoardTiles(IBoard board, Position startingPosition)
        {
            this.AppendExistingBoardTiles(
                board, startingPosition, x => new Position(x.Row - 1, x.Column), toFront: true);
        }

        public void AppendDownwardExistingBoardTiles(IBoard board, Position startingPosition)
        {
            this.AppendExistingBoardTiles(
                board, startingPosition, x => new Position(x.Row + 1, x.Column), toFront: false);
        }

        public void AppendNewTiles(IEnumerable<KeyValuePair<Tile, Position>> positionsByTiles, bool toFront = false)
        {
            foreach (var positionByTile in positionsByTiles)
            {
                if (toFront)
                {
                    this.AppendToFront(positionByTile);
                }
                else
                {
                    this.AppendToBack(positionByTile);
                }
            }
        }

        private void AppendExistingBoardTiles(
            IBoard board,
            Position startingPosition,
            Func<Position, Position> getNextPosition,
            bool toFront)
        {
            var current = new Position(startingPosition.Row, startingPosition.Column);

            while (true)
            {
                current = getNextPosition(current);

                if (!board.IsPositionInside(current) || board.IsCellFree(current))
                {
                    break;
                }

                var positionByTile = new KeyValuePair<Tile, Position>(board[current].Tile, current);

                if (toFront)
                {
                    this.AppendToFront(positionByTile);
                }
                else
                {
                    this.AppendToBack(positionByTile);
                }
            }
        }

        private void AppendToFront(KeyValuePair<Tile, Position> positionByTile)
        {
            this.positionsByTiles.AddFirst(positionByTile);
        }

        private void AppendToBack(KeyValuePair<Tile, Position> positionByTile)
        {
            this.positionsByTiles.AddLast(positionByTile);
        }
    }
}

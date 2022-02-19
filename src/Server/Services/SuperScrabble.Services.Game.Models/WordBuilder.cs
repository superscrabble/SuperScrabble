using System.Text;

using SuperScrabble.Common;
using SuperScrabble.Services.Game.Common;
using SuperScrabble.Services.Game.Models.Boards;

namespace SuperScrabble.Services.Game.Models;

public class WordBuilder
{
    private readonly LinkedList<KeyValuePair<Tile, Position>> _positionsByTiles = new();

    public IReadOnlyCollection<KeyValuePair<Tile, Position>> PositionsByTiles =>
        _positionsByTiles.ToList().AsReadOnly();

    public void AppendLeftwardExistingBoardTiles(IBoard board, Position startingPosition)
    {
        AppendExistingBoardTiles(
            board,
            startingPosition,
            x => new Position(x.Row, x.Column - 1),
            toFront: true,
            includeStartingPosition: true);
    }

    public void AppendRightwardExistingBoardTiles(IBoard board, Position startingPosition)
    {
        AppendExistingBoardTiles(
            board,
            startingPosition,
            x => new Position(x.Row, x.Column + 1),
            toFront: false,
            includeStartingPosition: false);
    }

    public void AppendUpwardExistingBoardTiles(IBoard board, Position startingPosition)
    {
        AppendExistingBoardTiles(
            board,
            startingPosition,
            x => new Position(x.Row - 1, x.Column),
            toFront: true,
            includeStartingPosition: true);
    }

    public void AppendDownwardExistingBoardTiles(IBoard board, Position startingPosition)
    {
        AppendExistingBoardTiles(
            board,
            startingPosition,
            x => new Position(x.Row + 1, x.Column),
            toFront: false,
            includeStartingPosition: false);
    }

    private void AppendExistingBoardTiles(
        IBoard board,
        Position startingPosition,
        Func<Position, Position> getNextPosition,
        bool toFront,
        bool includeStartingPosition)
    {
        var current = new Position(startingPosition.Row, startingPosition.Column);

        while (true)
        {
            if (includeStartingPosition)
            {
                includeStartingPosition = false;
            }
            else
            {
                current = getNextPosition(current);
            }

            if (!board.IsPositionInside(current) || board.IsCellFree(current))
            {
                break;
            }

            var positionByTile = new KeyValuePair<Tile, Position>(board[current].Tile!, current);

            if (toFront)
            {
                AppendToFront(positionByTile);
            }
            else
            {
                AppendToBack(positionByTile);
            }
        }
    }

    private void AppendToFront(KeyValuePair<Tile, Position> positionByTile)
    {
        _positionsByTiles.AddFirst(positionByTile);
    }

    private void AppendToBack(KeyValuePair<Tile, Position> positionByTile)
    {
        _positionsByTiles.AddLast(positionByTile);
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        foreach (var positionByTile in _positionsByTiles)
        {
            stringBuilder.Append(positionByTile.Key.Letter);
        }

        return stringBuilder.ToString();
    }
}

namespace SuperScrabble.Services.Game.Models
{
    using SuperScrabble.Common;

    public interface IBoard
    {
        int Height { get; }

        int Width { get; }

        Cell this[int row, int column] { get; set; }

        Cell this[Position position] { get; set; }

        bool IsCellFree(Position position);

        bool IsPositionInside(Position position);
    }
}

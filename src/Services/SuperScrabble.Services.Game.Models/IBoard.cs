namespace SuperScrabble.Services.Game.Models
{
    public interface IBoard
    {
        int Height { get; }

        int Width { get; }

        Cell this[int row, int column] { get; set; }
    }
}

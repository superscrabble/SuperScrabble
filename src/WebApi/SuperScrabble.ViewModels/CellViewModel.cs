namespace SuperScrabble.ViewModels
{
    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Models;

    public class CellViewModel
    {
        public Position Position { get; set; }

        public CellType Type { get; set; }

        public Tile Tile { get; set; }
    }
}

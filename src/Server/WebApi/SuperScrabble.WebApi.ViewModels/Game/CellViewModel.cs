namespace SuperScrabble.WebApi.ViewModels.Game
{
    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Common;

    public class CellViewModel
    {
        public Position Position { get; set; } = default!;

        public Tile Tile { get; set; } = default!;

        public CellType Type { get; set; }
    }
}
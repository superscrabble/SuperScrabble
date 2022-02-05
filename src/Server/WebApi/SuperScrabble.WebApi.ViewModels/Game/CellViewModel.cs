namespace SuperScrabble.WebApi.ViewModels.Game
{
    using SuperScrabble.Common;
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Common.Enums;

    public class CellViewModel
    {
        public Position Position { get; set; } = default!;

        public Tile? Tile { get; set; }

        public CellType Type { get; set; }
    }
}
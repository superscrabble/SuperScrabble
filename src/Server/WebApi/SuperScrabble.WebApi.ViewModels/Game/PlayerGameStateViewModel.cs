namespace SuperScrabble.WebApi.ViewModels.Game
{
    using SuperScrabble.Services.Game.Common;

    public class PlayerGameStateViewModel
    {
        public IEnumerable<Tile> Tiles { get; set; } = default!;

        public CommonGameStateViewModel CommonGameState { get; set; } = default!;

        public string MyUserName { get; set; } = default!;
    }
}

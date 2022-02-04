namespace SuperScrabble.WebApi.ViewModels.Game
{
    using SuperScrabble.Services.Game.Common;

    public class ExchangeTilesInputModel
    {
        public IEnumerable<Tile> TilesToExchange { get; set; } = default!;
    }
}

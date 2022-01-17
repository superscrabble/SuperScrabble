namespace SuperScrabble.Services.Game
{
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.WebApi.ViewModels.Game;

    public interface IGameService
    {
        PlayerGameStateViewModel MapFromGameState(GameState gameState, string userName);

        void FillPlayerTiles(GameState gameState, string userName);

        GameOperationResult WriteWord(
            GameState gameState, WriteWordInputModel input, string authorUserName);

        GameOperationResult ExchangeTiles(
            GameState gameState, ExchangeTilesInputModel input, string exchangerUserName);

        GameOperationResult SkipTurn(GameState gameState, string skipperUserName);
    }
}

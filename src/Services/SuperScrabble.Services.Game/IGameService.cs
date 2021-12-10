namespace SuperScrabble.Services.Game
{
    using System.Collections.Generic;

    using SuperScrabble.ViewModels;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Services.Game.Models;

    public interface IGameService
    {
        GameState CreateGame(IEnumerable<KeyValuePair<string, string>> connectionIdsByUserNames);

        PlayerGameStateViewModel MapFromGameState(GameState gameState, string userName);

        void FillPlayerTiles(GameState gameState, string userName);

        WriteWordResult WriteWord(GameState gameState, WriteWordInputModel input, string authorUserName);
    }
}

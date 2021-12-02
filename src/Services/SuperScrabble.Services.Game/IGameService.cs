namespace SuperScrabble.Services.Game
{
    using System.Collections.Generic;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.ViewModels;

    public interface IGameService
    {
        GameState CreateGame(IEnumerable<KeyValuePair<string, string>> connectionIdsByUserNames);

        PlayerGameStateViewModel MapFromGameState(GameState gameState, string userName);

        void FillPlayerTiles(GameState gameState, string userName);

        void WriteWord(GameState gameState, WriteWordInputModel input, string authorUserName);
    }
}

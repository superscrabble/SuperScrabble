using SuperScrabble.Data;
using SuperScrabble.Data.Repositories;
using SuperScrabble.Models;
using SuperScrabble.Services;
using SuperScrabble.Services.Data;
using SuperScrabble.Services.Game;
using SuperScrabble.Services.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperScrabble.SandBox
{
    public class Program
    {
        public static async Task Main()
        {
            var gameService = new GameService(
                new ShuffleService(),
                new MyOldBoardTilesProvider(),
                new MyOldBoardBonusCellsProvider(),
                new StandardGameplayConstantsProvider(),
                null);

            GameState gameState = gameService.CreateGame(
                new KeyValuePair<string, string>[] { new("Denis", "123456780"), new("Georgi", "123456789") });

            foreach (var player in gameState.Players)
            {
                gameService.FillPlayerTiles(gameState, player.UserName);
            }

            gameState.Board[7, 7].Tile = new Tile('К', 2);
            gameState.Board[8, 7].Tile = new Tile('О', 1);
            gameState.Board[9, 7].Tile = new Tile('Н', 1);


        }
    }
}

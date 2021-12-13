using SuperScrabble.Common;
using SuperScrabble.Data;
using SuperScrabble.Data.Repositories;
using SuperScrabble.InputModels.Game;
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
            gameState.Board[7, 8].Tile = new Tile('О', 1);
            gameState.Board[7, 9].Tile = new Tile('Н', 1);

            var input = new WriteWordInputModel
            {
                PositionsByTiles = new List<KeyValuePair<Tile, Position>>
                {
                    new(gameState.Players.First(p => p.UserName == "Georgi").GetTile(0), new Position(6, 8)),
                    new(gameState.Players.First(p => p.UserName == "Georgi").GetTile(1), new Position(8, 8)),
                },
            };

            gameService.WriteWord(gameState, input, "Georgi");

            for (int row = 0; row < gameState.Board.Height; row++)
            {
                for (int col = 0; col < gameState.Board.Width; col++)
                {

                    if (gameState.Board[row, col].Tile == null)
                    {
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(gameState.Board[row, col].Tile.Letter);
                    }
                }

                Console.WriteLine();
            }
        }
    }
}

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
        public static void Main()
        {
            var player = new Player("sss", 0, "sss");
            player.AddTile(new Tile('А', 8));
            Console.WriteLine(player.Tiles.Count);
            player.RemoveTile(new Tile('А', 8));
            Console.WriteLine(player.Tiles.Count);
        }
    }
}

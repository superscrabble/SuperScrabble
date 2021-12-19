using Microsoft.EntityFrameworkCore;
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
            var db = new AppDbContext();
            var ivan = await db.Users.FirstOrDefaultAsync(u => u.UserName == "Ivan2");
            Console.WriteLine(ivan.Email);
        }
    }
}

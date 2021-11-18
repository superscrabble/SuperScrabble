using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.Services.Game
{
    public class GameState
    {
        private readonly List<string> userNames = new List<string>();

        public GameState(IEnumerable<string> userNames)
        {
            this.userNames.AddRange(userNames);
        }
    }
}

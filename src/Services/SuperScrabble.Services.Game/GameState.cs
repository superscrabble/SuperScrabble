using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.Services.Game
{
    public class GameState
    {
        public int PlayerIndex { get; private set; }
        private readonly List<Player> players = new();
        private readonly TilesBag tilesBag = new();

        public GameState(IEnumerable<string> userNames)
        {
            var shuffledUserNames = Shuffle(userNames);

            foreach (var userName in shuffledUserNames)
            {
                var player = new Player { UserName = userName, Points = 0 };
                this.players.Add(player);
            }
            PlayerIndex = 0;
        }

        private IEnumerable<T> Shuffle<T>(IEnumerable<T> items)
        {
            var random = new Random();
            var list = items.ToList();

            var result = new List<T>();
            while (list.Count > 0)
            {
                var index = random.Next(list.Count);
                result.Add(list[index]);
                list.RemoveAt(index);
            }

            return result;
        }

        public void NextPlayer()
        {
            PlayerIndex = PlayerIndex >= userNames.Count ? 0 : PlayerIndex + 1;
        }
    }
}

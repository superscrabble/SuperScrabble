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
        private readonly List<string> userNames = new();

        public GameState(IEnumerable<string> userNames)
        {
            this.userNames.AddRange(Shuffle(userNames));
        }

        private IEnumerable<string> Shuffle(IEnumerable<string> items)
        {
            var random = new Random();
            var list= items.ToList();
            
            var result = new List<string>();
            while(list.Count > 0)
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

namespace SuperScrabble.Services.Game.Models
{
    using System.Linq;
    using System.Collections.Generic;

    public class GameState
    {
        private readonly List<Player> players = new();

        public GameState(IEnumerable<KeyValuePair<string, string>> connectionIdsByUserNames, TilesBag tilesBag)
        {
            foreach (var user in connectionIdsByUserNames)
            {
                this.players.Add(new Player(user.Key, 0, user.Value));
            }

            this.TilesBag = tilesBag;
            this.PlayerIndex = 0;
        }

        public TilesBag TilesBag { get; }

        public int PlayerIndex { get; private set; }

        public IReadOnlyCollection<Player> Players => this.players.AsReadOnly();

        public int TilesCount => this.TilesBag.TilesCount;

        public Player GetPlayer(string userName)
        {
            return this.players.FirstOrDefault(p => p.UserName == userName);
        }

        public void NextPlayer()
        {
            this.PlayerIndex = this.PlayerIndex >= this.players.Count ? 0 : this.PlayerIndex + 1;
        }
    }
}

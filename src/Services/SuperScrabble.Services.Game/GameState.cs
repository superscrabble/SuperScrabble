namespace SuperScrabble.Services.Game
{
    using System.Collections.Generic;

    public class GameState
    {
        private readonly TilesBag tilesBag;
        private readonly List<Player> players = new();

        public GameState(IEnumerable<string> userNames, TilesBag tilesBag)
        {
            foreach (string userName in userNames)
            {
                this.players.Add(new Player(userName, 0));
            }

            this.tilesBag = tilesBag;
            this.PlayerIndex = 0;
        }

        public int PlayerIndex { get; private set; }

        public void NextPlayer()
        {
            this.PlayerIndex = this.PlayerIndex >= this.players.Count ? 0 : this.PlayerIndex + 1;
        }
    }
}

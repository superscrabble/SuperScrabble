namespace SuperScrabble.Services.Game.Models
{
    public class Team
    {
        private readonly List<Player> players = new();

        public Team(int maxPlayersCount)
        {
            this.MaxPlayersCount = maxPlayersCount;
            this.PlayerIndex = 0;
            this.IsTurnFinished = false;
        }
        public int MaxPlayersCount { get; }

        public int PlayerIndex { get; private set; }

        public bool IsTurnFinished { get; private set; }

        public Player CurrentPlayer => this.players[this.PlayerIndex];

        public bool HasSurrendered => this.players.All(pl => pl.HasLeftTheGame);

        public IEnumerable<Player> Players => this.players.AsReadOnly();

        public void ResetConsecutiveSkipsCount()
        {
            foreach (Player player in this.players)
            {
                player.ConsecutiveSkipsCount = 0;
            }
        }

        public void NextPlayer()
        {
            this.IsTurnFinished = false;

            while (this.players.Count >= 1)
            {
                this.PlayerIndex++;

                if (this.PlayerIndex >= this.players.Count)
                {
                    this.IsTurnFinished = true;
                    this.PlayerIndex = 0;
                    break;
                }

                if (!this.CurrentPlayer.HasLeftTheGame)
                {
                    break;
                }
            }
        }

        public bool AddPlayer(string userName, string connectionId)
        {
            bool isPlayerAlreadyAdded = this.players.Any(
                pl => pl.UserName == userName || pl.ConnectionId == connectionId);

            if (isPlayerAlreadyAdded)
            {
                return false;
            }

            this.players.Add(new Player(userName, connectionId));
            return true;
        }
    }
}

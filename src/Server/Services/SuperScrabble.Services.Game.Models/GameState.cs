namespace SuperScrabble.Services.Game.Models
{
    using SuperScrabble.Services.Game.Common.GameplayConstantsProviders;

    using SuperScrabble.Services.Game.Models.Bags;
    using SuperScrabble.Services.Game.Models.Boards;

    public class GameState
    {
        private readonly List<Team> _teams = new();

        public GameState(
            IBag bag,
            IBoard board,
            string groupName,
            IEnumerable<Team> teams,
            IGameplayConstantsProvider gameplayConstantsProvider)
        {
            _teams.AddRange(teams);

            Bag = bag;
            Board = board;
            GameId = groupName;
            TeamIndex = 0;
            IsGameOver = false;
            GameplayConstants = gameplayConstantsProvider;
        }

        public Dictionary<string, int> RemainingSecondsByUserNames { get; } = new();

        public IGameplayConstantsProvider GameplayConstants { get; set; }

        public IBag Bag { get; }

        public IBoard Board { get; }

        public string GameId { get; }

        public int TeamIndex { get; private set; }

        public bool IsGameOver { get; private set; }

        public IReadOnlyCollection<Team> Teams => this._teams.AsReadOnly();

        public IReadOnlyCollection<Player> Players =>
            this._teams.SelectMany(team => team.Players).ToList().AsReadOnly();

        public Team CurrentTeam => this._teams[this.TeamIndex];

        public int TilesCount => this.Bag.TilesCount;

        public bool IsTileExchangePossible =>
            this.TilesCount >= this.GameplayConstants.PlayerTilesCount;

        public void EndGame()
        {
            this.IsGameOver = true;
        }

        public void EndGameIfRoomIsEmpty()
        {
            int remainingTeamsCount = 0;

            foreach (Team team in this._teams)
            {
                if (!team.HasSurrendered
                    && HasPlayerTime(team.CurrentPlayer.UserName))
                {
                    remainingTeamsCount++;
                    break;
                }
            }

            if (remainingTeamsCount <= 1)
            {
                EndGame();
            }
        }

        public void ResetConsecutiveSkipsCount()
        {
            foreach (Team team in this._teams)
            {
                team.ResetConsecutiveSkipsCount();
            }
        }

        public void NextTeam()
        {
            while (_teams.Count > 1)
            {
                TeamIndex++;

                if (TeamIndex >= _teams.Count)
                {
                    TeamIndex = 0;
                }

                if (!CurrentTeam.HasSurrendered
                    && HasPlayerTime(CurrentTeam.CurrentPlayer.UserName))
                {
                    break;
                }
            }
        }

        private bool HasPlayerTime(string userName)
        {
            if (RemainingSecondsByUserNames.Count == 0)
            {
                return true;
            }

            return RemainingSecondsByUserNames[userName] > 0;
        }

        public Player? GetPlayer(string userName)
        {
            foreach (Team team in this.Teams)
            {
                Player? player = team.Players.FirstOrDefault(pl => pl.UserName == userName);

                if (player != null)
                {
                    return player;
                }
            }

            return null;
        }

        public Team? GetTeam(string userName)
        {
            return this.Teams.FirstOrDefault(
                team => team.Players.Any(pl => pl.UserName == userName));
        }

        public IEnumerable<string> GetUserNamesOfPlayersWhoHaveLeftTheGame()
        {
            var playersWhoHaveLeft = new List<string>();

            foreach (Team team in this.Teams)
            {
                playersWhoHaveLeft.AddRange(team.Players
                    .Where(pl => pl.HasLeftTheGame).Select(pl => pl.UserName));
            }

            return playersWhoHaveLeft;
        }
    }
}

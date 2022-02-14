namespace SuperScrabble.Services.Game.Models
{
    using SuperScrabble.Services.Game.Common.GameplayConstantsProviders;

    using SuperScrabble.Services.Game.Models.Bags;
    using SuperScrabble.Services.Game.Models.Boards;

    public class GameState
    {
        private readonly List<Team> teams = new();

        public GameState(
            IBag bag,
            IBoard board,
            string groupName,
            IEnumerable<Team> teams,
            IGameplayConstantsProvider gameplayConstantsProvider)
        {
            this.teams.AddRange(teams);

            this.Bag = bag;
            this.Board = board;
            this.GameId = groupName;
            this.TeamIndex = 0;
            this.IsGameOver = false;
            this.GameplayConstants = gameplayConstantsProvider;
        }

        public IGameplayConstantsProvider GameplayConstants { get; set; }

        public IBag Bag { get; }

        public IBoard Board { get; }

        public string GameId { get; }

        public int TeamIndex { get; private set; }

        public bool IsGameOver { get; private set; }

        public IReadOnlyCollection<Team> Teams => this.teams.AsReadOnly();

        public IReadOnlyCollection<Player> Players =>
            this.teams.SelectMany(team => team.Players).ToList().AsReadOnly();

        public Team CurrentTeam => this.teams[this.TeamIndex];

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

            foreach (Team team in this.teams)
            {
                if (!team.HasSurrendered)
                {
                    remainingTeamsCount++;
                    break;
                }
            }

            if (remainingTeamsCount <= 1)
            {
                this.EndGame();
            }
        }

        public void ResetConsecutiveSkipsCount()
        {
            foreach (Team team in this.teams)
            {
                team.ResetConsecutiveSkipsCount();
            }
        }

        public void NextTeam()
        {
            while (this.teams.Count > 1)
            {
                this.TeamIndex++;

                if (this.TeamIndex >= this.teams.Count)
                {
                    this.TeamIndex = 0;
                }

                if (!this.CurrentTeam.HasSurrendered)
                {
                    break;
                }
            }
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

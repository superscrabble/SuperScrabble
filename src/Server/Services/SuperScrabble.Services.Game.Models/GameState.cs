namespace SuperScrabble.Services.Game.Models
{
    using SuperScrabble.Services.Game.Common.GameplayConstantsProviders;

    using SuperScrabble.Services.Game.Models.Bags;
    using SuperScrabble.Services.Game.Models.Boards;

    public class GameState
    {
        private readonly List<Team> teams = new();
        private readonly IGameplayConstantsProvider gameplayConstantsProvider;

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
            this.GroupName = groupName;
            this.TeamIndex = 0;
            this.IsGameOver = false;
            this.gameplayConstantsProvider = gameplayConstantsProvider;
        }

        public IBag Bag { get; }

        public IBoard Board { get; }

        public string GroupName { get; }

        public int TeamIndex { get; private set; }

        public bool IsGameOver { get; private set; }

        public IReadOnlyCollection<Team> Teams => this.teams.AsReadOnly();

        public Team CurrentTeam => this.teams[this.TeamIndex];

        public int TilesCount => this.Bag.TilesCount;

        public bool IsTileExchangePossible =>
            this.TilesCount >= this.gameplayConstantsProvider.PlayerTilesCount;

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
    }
}

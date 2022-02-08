namespace SuperScrabble.Services.Game.Factories
{
    using SuperScrabble.Services.Common;
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Common.BonusCellsProviders;
    using SuperScrabble.Services.Game.Common.Enums;
    using SuperScrabble.Services.Game.Common.GameplayConstantsProviders;
    using SuperScrabble.Services.Game.Common.TilesProviders;

    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.Models.Bags;
    using SuperScrabble.Services.Game.Models.Boards;

    public class GameStateFactory : IGameStateFactory
    {
        private readonly IShuffleService shuffleService;

        public GameStateFactory(IShuffleService shuffleService)
        {
            this.shuffleService = shuffleService;
        }

        public GameState CreateGameState(
            GameRoomConfiguration roomConfiguration, IEnumerable<Team> teams, string groupName)
        {
            var bonusCellsProvider = new StandardBonusCellsProvider();
            IBoard board = new StandardBoard(bonusCellsProvider);

            var gameplayConstantsProvider = new StandardGameplayConstantsProvider(
                    roomConfiguration.TeamsCount,
                    roomConfiguration.TimerDifficulty.GetSeconds(roomConfiguration.TimerType));

            var shuffledTilesProvider = new ShuffledTilesProvider(
                    this.shuffleService, new StandardTilesProvider());

            IBag bag = new Bag(shuffledTilesProvider);

            return new GameState(bag, board, groupName, teams, gameplayConstantsProvider);
        }

        public GameState CreateGameState(GameMode gameMode, IEnumerable<WaitingTeam> waitingTeams, string groupName)
        {
            GameRoomConfiguration config = default!;

            if (gameMode == GameMode.Duo)
            {
                config = new GameRoomConfiguration
                {
                    TeamsCount = 2,
                    TeamType = TeamType.Duo,
                    TimerDifficulty = TimerDifficulty.Normal,
                    TimerType = TimerType.Standard,
                };
            }

            var teams = waitingTeams.Select(wt =>
            {
                var team = new Team();

                foreach (Member member in wt.Members)
                {
                    team.AddPlayer(member.UserName, member.ConnectionId);
                }

                return team;
            });

            return this.CreateGameState(config, teams, groupName);
        }

        private IBoard CreateBoard(BoardType boardType, IBonusCellsProvider bonusCellsProvider)
        {
            return boardType switch
            {
                BoardType.Standard => new StandardBoard(bonusCellsProvider),
                BoardType.Giant => new GiantBoard(bonusCellsProvider),
                _ => throw new NotSupportedException($"Not supported {nameof(boardType)}."),
            };
        }
    }
}

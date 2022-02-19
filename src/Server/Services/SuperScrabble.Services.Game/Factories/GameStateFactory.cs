namespace SuperScrabble.Services.Game.Factories
{
    using Microsoft.Extensions.DependencyInjection;
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
        private readonly IShuffleService _shuffleService;

        public GameStateFactory(IShuffleService shuffleService)
        {
            _shuffleService = shuffleService;
        }

        public GameState CreateGameState(
            GameRoomConfiguration config, IEnumerable<Team> teams, string gameId)
        {
            var bonusCellsProvider = new StandardBonusCellsProvider();
            IBoard board = new StandardBoard(bonusCellsProvider);

            var gameplayConstants = new StandardGameplayConstants(
                    config.TeamsCount,
                    config.TimerDifficulty.GetSeconds(config.TimerType));

            var shuffledTilesProvider = new ShuffledTilesProvider(
                    _shuffleService, new StandardTilesProvider());

            IBag bag = new Bag(shuffledTilesProvider);

            var gameState = new GameState(bag, board, gameId, teams, gameplayConstants);

            foreach (var player in gameState.Players)
            {
                gameState.RemainingSecondsByUserNames.Add(
                    player.UserName, gameState.GameplayConstants.GameTimerSeconds);
            }

            return gameState;
        }

        public GameState CreateGameState(
            GameMode gameMode, IEnumerable<WaitingTeam> waitingTeams, string groupName)
        {
            var config = new GameRoomConfiguration
            {
                TeamType = TeamType.Solo,
                TeamsCount = gameMode.GetTeamsCount(),
                TimerType = TimerType.Standard,
                TimerDifficulty = TimerDifficulty.Normal,
            };

            if (gameMode == GameMode.Duo)
            {
                config.TeamType = TeamType.Duo;
            }

            if (gameMode == GameMode.ChessScrabble)
            {
                config.TimerType = TimerType.Chess;
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

            return CreateGameState(config, teams, groupName);
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

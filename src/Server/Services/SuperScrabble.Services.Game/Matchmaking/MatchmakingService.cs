namespace SuperScrabble.Services.Game.Matchmaking
{
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Factories;
    using SuperScrabble.Services.Game.Models;

    public class MatchmakingService : IMatchmakingService
    {
        private static readonly Dictionary<string, string> connectionIdsByUserName = new();

        private static readonly Dictionary<string, string> groupNamesByUserName = new();
        private static readonly Dictionary<string, GameState> gameStatesByGroupName = new();

        private static readonly Dictionary<
            GameRoomConfiguration, List<Team>> waitingTeamsByGameRoomConfiguration = new();

        private readonly IGameStateFactory gameStateFactory;

        public MatchmakingService(IGameStateFactory gameStateFactory)
        {
            this.gameStateFactory = gameStateFactory;
        }

        public void AddTeamToWaitingQueue(
            GameRoomConfiguration roomConfiguration, Team teamToAdd, out bool hasGameStarted)
        {
            hasGameStarted = false;

            if (!waitingTeamsByGameRoomConfiguration.ContainsKey(roomConfiguration))
            {
                waitingTeamsByGameRoomConfiguration.Add(roomConfiguration, new());
            }

            bool isRoomFull = waitingTeamsByGameRoomConfiguration[roomConfiguration].Count
                >= roomConfiguration.TeamsCount;

            if (!isRoomFull)
            {
                waitingTeamsByGameRoomConfiguration[roomConfiguration].Add(teamToAdd);
                return;
            }

            string groupName = Guid.NewGuid().ToString();
            var teams = waitingTeamsByGameRoomConfiguration[roomConfiguration];

            GameState gameState = this.gameStateFactory.CreateGameState(roomConfiguration, teams, groupName);

            waitingTeamsByGameRoomConfiguration.Remove(roomConfiguration);
            gameStatesByGroupName.Add(groupName, gameState);

            foreach (Team team in gameState.Teams)
            {
                foreach (Player player in team.Players)
                {
                    groupNamesByUserName.Add(player.UserName, groupName);
                }
            }

            hasGameStarted = true;
        }
    }
}

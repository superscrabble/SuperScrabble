namespace SuperScrabble.Services.Game.Matchmaking
{
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Factories;
    using SuperScrabble.Services.Game.Models;

    public class MatchmakingService : IMatchmakingService
    {
        private static readonly Dictionary<string, string> groupNamesByUserNames = new();
        private static readonly Dictionary<string, GameState> gameStatesByGroupNames = new();

        private static readonly Dictionary<string, Lobby> friendlyLobbiesByLobbyIds = new();

        private static readonly Dictionary<
            GameRoomConfiguration, List<Team>> waitingTeamsByRoomConfigs = new();

        private readonly IGameStateFactory gameStateFactory;

        public MatchmakingService(IGameStateFactory gameStateFactory)
        {
            this.gameStateFactory = gameStateFactory;
        }

        public void AddTeamToWaitingQueue(
            GameRoomConfiguration roomConfiguration, Team teamToAdd, out bool hasGameStarted)
        {
            hasGameStarted = false;

            if (!waitingTeamsByRoomConfigs.ContainsKey(roomConfiguration))
            {
                waitingTeamsByRoomConfigs.Add(roomConfiguration, new());
            }

            waitingTeamsByRoomConfigs[roomConfiguration].Add(teamToAdd);

            bool isRoomFull = waitingTeamsByRoomConfigs[
                roomConfiguration].Count == roomConfiguration.TeamsCount;

            if (!isRoomFull)
            {
                return;
            }

            string groupName = Guid.NewGuid().ToString();
            var teams = waitingTeamsByRoomConfigs[roomConfiguration];

            GameState gameState = this.gameStateFactory.CreateGameState(
                roomConfiguration, teams, groupName);

            waitingTeamsByRoomConfigs.Remove(roomConfiguration);
            gameStatesByGroupNames.Add(groupName, gameState);

            foreach (Team team in gameState.Teams)
            {
                foreach (Player player in team.Players)
                {
                    groupNamesByUserNames.Add(player.UserName, groupName);
                }
            }

            hasGameStarted = true;
        }

        public void AddPlayerToLobby(
            GameRoomConfiguration roomConfiguration, Player player, out bool isLobbyReady)
        {
            throw new NotImplementedException();
        }

        public GameState? GetGameState(string userName)
        {
            if (!groupNamesByUserNames.ContainsKey(userName))
            {
                return null;
            }

            string groupName = groupNamesByUserNames[userName];
            return gameStatesByGroupNames[groupName];
        }

        public bool IsUserAlreadyInsideGame(string userName)
        {
            return groupNamesByUserNames.ContainsKey(userName);
        }

        public bool IsUserAlreadyWaitingToJoinGame(string userName)
        {
            foreach (var teamByRoomConfig in waitingTeamsByRoomConfigs)
            {
                foreach (Team team in teamByRoomConfig.Value)
                {
                    if (team.Players.Any(pl => pl.UserName == userName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

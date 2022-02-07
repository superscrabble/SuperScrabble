namespace SuperScrabble.Services.Game.Matchmaking
{
    using SuperScrabble.Common.Exceptions.Matchmaking;
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Common.Enums;
    using SuperScrabble.Services.Game.Factories;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.WebApi.ViewModels.Game;

    public class MatchmakingService : IMatchmakingService
    {
        class FriendlyGameLobby
        {
            public FriendlyGameLobby(Player owner, CreateFriendlyGameInputModel input)
            {
                this.Owner = owner;
                this.Input = input;
            }

            public Player Owner { get; private set; }

            public List<Player> OtherPlayers { get; } = new();

            public CreateFriendlyGameInputModel Input { get; }
        }

        private static readonly Dictionary<
            string, FriendlyGameLobby> friendlyGameLobbiesByInvitationCodes = new();

        private static readonly Dictionary<string, string> groupNamesByUserNames = new();
        private static readonly Dictionary<string, GameState> gameStatesByGroupNames = new();

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

        public string CreateFriendlyGame(
            string creatorUserName, string creatorConnectionId, CreateFriendlyGameInputModel input)
        {
            var random = new Random();

            while (true)
            {
                string invitationCode = random.Next(1_000, 10_000).ToString();

                if (!friendlyGameLobbiesByInvitationCodes.ContainsKey(invitationCode))
                {
                    var gameLobby = new FriendlyGameLobby(new Player(creatorUserName, creatorConnectionId), input);
                    friendlyGameLobbiesByInvitationCodes.Add(invitationCode, gameLobby);
                    return invitationCode;
                }
            }
        }

        public void JoinFriendlyGame(string joinerName, string joinerConnectionId, string invitationCode, out bool canGameBeStarted)
        {
            const int MaxPlayersCount = 4;
            const int MinPlayersCount = 2;

            bool isInvitationCodeExisting =
                friendlyGameLobbiesByInvitationCodes.ContainsKey(invitationCode);

            if (!isInvitationCodeExisting)
            {
                throw new UnexistingInvitationCodeException();
            }

            var gameLobby = friendlyGameLobbiesByInvitationCodes[invitationCode];

            gameLobby.OtherPlayers.Add(new Player(joinerName, joinerConnectionId));

            int playersCount = gameLobby.OtherPlayers.Count + 1;

            bool isGameLobbyFull = playersCount >= MaxPlayersCount;

            if (isGameLobbyFull)
            {
                throw new GameLobbyFullException();
            }

            if (playersCount < MinPlayersCount)
            {
                canGameBeStarted = false;
                return;
            }

            gameLobby.OtherPlayers.Add(gameLobby.Owner);

            var teams = gameLobby.OtherPlayers.Select(pl =>
            {
                var team = new Team(1);
                team.AddPlayer(pl.UserName, pl.ConnectionId);
                return team;
            });

            string groupName = Guid.NewGuid().ToString();

            var gameState = this.gameStateFactory.CreateGameState(
                new GameRoomConfiguration
                {
                    TeamsCount = playersCount,
                    TeamType = TeamType.Solo,
                    TimerDifficulty = gameLobby.Input.TimerDifficulty,
                    TimerType = gameLobby.Input.TimerType,
                },
                teams,
                groupName);

            gameStatesByGroupNames.Add(groupName, gameState);

            friendlyGameLobbiesByInvitationCodes.Remove(invitationCode);

            foreach (Team team in gameState.Teams)
            {
                foreach (Player player in team.Players)
                {
                    groupNamesByUserNames.Add(player.UserName, groupName);
                }
            }

            canGameBeStarted = true;
        }
    }
}

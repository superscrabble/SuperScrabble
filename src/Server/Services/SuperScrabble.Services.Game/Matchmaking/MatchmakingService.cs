namespace SuperScrabble.Services.Game.Matchmaking
{
    using SuperScrabble.Common.Exceptions.Matchmaking;
    using SuperScrabble.Services.Common;
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Common.Enums;
    using SuperScrabble.Services.Game.Factories;
    using SuperScrabble.Services.Game.Models;
    using System.Collections.Concurrent;

    public class MatchmakingService : IMatchmakingService
    {
        private static readonly ConcurrentDictionary<
            string, FriendlyGameLobby> friendlyGameLobbiesByInvitationCodes = new();

        private static readonly ConcurrentDictionary<string, string> groupNamesByUserNames = new();

        private static readonly ConcurrentDictionary<string, GameState> gameStatesByGroupNames = new();

        private static readonly Dictionary<
            GameRoomConfiguration, List<Team>> waitingTeamsByRoomConfigs = new();

        private readonly IGameStateFactory gameStateFactory;
        private readonly IInvitationCodeGenerator invitationCodeGenerator;

        public MatchmakingService(
            IGameStateFactory gameStateFactory,
            IInvitationCodeGenerator invitationCodeGenerator)
        {
            this.gameStateFactory = gameStateFactory;
            this.invitationCodeGenerator = invitationCodeGenerator;
        }

        //check if user is already in lobby or is inside game

        public string CreateFriendlyGame(string creatorUserName,
            string creatorConnectionId, FriendlyGameConfiguration gameConfig)
        {
            while (true)
            {
                string invitationCode = this.invitationCodeGenerator.GenerateInvitationCode();

                if (!friendlyGameLobbiesByInvitationCodes.ContainsKey(invitationCode))
                {
                    var gameLobby = new FriendlyGameLobby(
                        new Player(creatorUserName, creatorConnectionId), gameConfig);

                    friendlyGameLobbiesByInvitationCodes.TryAdd(invitationCode, gameLobby);
                    return invitationCode;
                }
            }
        }

        public void JoinFriendlyGame(string joinerUserName, string joinerConnectionId, string invitationCode)
        {
            ThrowIfInvitationCodeIsNotExisting(invitationCode);

            var gameLobby = friendlyGameLobbiesByInvitationCodes[invitationCode];

            bool isPlayerAlreadyInsideLobby = gameLobby.LobbyMembers
                .Any(mem => mem.UserName == joinerUserName);

            if (isPlayerAlreadyInsideLobby)
            {
                throw new PlayerAlreadyInsideLobbyException();
            }

            if (gameLobby.IsFull)
            {
                throw new GameLobbyFullException();
            }

            gameLobby.LobbyMembers.Add(new Player(joinerUserName, joinerConnectionId));
        }

        public void StartFriendlyGame(string starterUserName, string invitationCode)
        {
            ThrowIfInvitationCodeIsNotExisting(invitationCode);

            var gameLobby = this.GetFriendlyGameLobby(invitationCode);

            if (!gameLobby.IsAbleToStartGame)
            {
                throw new NotEnoughPlayersToStartFriendlyGameException();
            }

            if (gameLobby.Owner.UserName != starterUserName)
            {
                throw new UnauthorizedToStartGameException();
            }

            var teams = gameLobby.LobbyMembers.Select(pl =>
            {
                var team = new Team();
                team.AddPlayer(pl.UserName, pl.ConnectionId);
                return team;
            });

            string groupName = Guid.NewGuid().ToString();

            var gameState = this.gameStateFactory.CreateGameState(
                new GameRoomConfiguration
                {
                    TeamsCount = teams.Count(),
                    TeamType = TeamType.Solo,
                    TimerDifficulty = gameLobby.GameConfig.TimerDifficulty,
                    TimerType = gameLobby.GameConfig.TimerType,
                },
                teams,
                groupName);

            gameStatesByGroupNames.TryAdd(groupName, gameState);
            
            friendlyGameLobbiesByInvitationCodes.TryRemove(new(invitationCode, gameLobby));

            foreach (Team team in gameState.Teams)
            {
                foreach (Player player in team.Players)
                {
                    groupNamesByUserNames.TryAdd(player.UserName, groupName);
                }
            }
        }

        public FriendlyGameLobby GetFriendlyGameLobby(string invitationCode)
        {
            if (!friendlyGameLobbiesByInvitationCodes.ContainsKey(invitationCode))
            {
                throw new ArgumentException($"{nameof(FriendlyGameLobby)} with such {invitationCode} was not found.");
            }

            return friendlyGameLobbiesByInvitationCodes[invitationCode];
        }

        public GameState GetGameState(string userName)
        {
            if (!groupNamesByUserNames.ContainsKey(userName))
            {
                throw new ArgumentException($"No {nameof(GameState)} for the given {nameof(userName)} was found.");
            }

            string groupName = groupNamesByUserNames[userName];
            return gameStatesByGroupNames[groupName];
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
            gameStatesByGroupNames.TryAdd(groupName, gameState);

            foreach (Team team in gameState.Teams)
            {
                foreach (Player player in team.Players)
                {
                    groupNamesByUserNames.TryAdd(player.UserName, groupName);
                }
            }

            hasGameStarted = true;
        }

        public void AddPlayerToLobby(
            GameRoomConfiguration roomConfiguration, Player player, out bool isLobbyReady)
        {
            throw new NotImplementedException();
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

        private static void ThrowIfInvitationCodeIsNotExisting(string invitationCode)
        {
            bool isInvitationCodeExisting = friendlyGameLobbiesByInvitationCodes.ContainsKey(invitationCode);

            if (!isInvitationCodeExisting)
            {
                throw new UnexistingInvitationCodeException();
            }
        }
    }
}

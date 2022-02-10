namespace SuperScrabble.Services.Game.Matchmaking
{
    using System.Collections.Concurrent;

    using SuperScrabble.Common.Exceptions.Matchmaking;
    using SuperScrabble.Common.Exceptions.Matchmaking.Party;

    using SuperScrabble.Services.Common;
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Common.Enums;
    using SuperScrabble.Services.Game.Factories;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.Models.Parties;

    public class MatchmakingService : IMatchmakingService
    {
        private static readonly ConcurrentDictionary<string, string> partyIdsByInvitationCodes = new();

        private static readonly ConcurrentDictionary<string, Party> partiesByPartyIds = new();

        private static readonly ConcurrentDictionary<
            GameMode, List<WaitingTeam>> waitingTeamsByGameModes = new();

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

        public string CreateParty(string creatorUserName, string creatorConnectionId, PartyType partyType)
        {
            while (true)
            {
                string invitationCode = this.invitationCodeGenerator.GenerateInvitationCode();

                if (!partyIdsByInvitationCodes.ContainsKey(invitationCode))
                {
                    Party party = default!;

                    var owner = new Member(creatorUserName, creatorConnectionId);
                    var partyId = Guid.NewGuid().ToString();

                    if (partyType == PartyType.Friendly)
                    {
                        party = new FriendParty(owner, partyId, invitationCode);
                    }
                    else if (partyType == PartyType.Duo)
                    {
                        party = new DuoParty(owner, partyId, invitationCode);
                    }

                    partyIdsByInvitationCodes.TryAdd(invitationCode, partyId);
                    partiesByPartyIds.TryAdd(partyId, party);

                    return partyId;
                }
            }
        }

        public void JoinParty(
            string joinerUserName, string joinerConnectionId,
            string invitationCode, out bool hasEnoughPlayersToStartGame)
        {
            ThrowIfInvitationCodeIsNotExisting(invitationCode);
            Party party = this.GetPartyByInvitationCode(invitationCode);
            party.AddMember(new Member(joinerUserName, joinerConnectionId));
            hasEnoughPlayersToStartGame = party.HasEnoughPlayersToStartGame;
        }

        public void StartGameFromParty(string starterUserName, string partyId, out bool hasGameStarted)
        {
            Party party = GetPartyById(partyId);
            hasGameStarted = false;

            if (!party.HasEnoughPlayersToStartGame)
            {
                throw new NotEnoughPlayersToStartGameException();
            }

            if (party.Owner?.UserName != starterUserName)
            {
                throw new OnlyOwnerHasAccessException();
            }

            if (party is FriendParty friendParty)
            {
                var teams = party.Members.Select(mem =>
                {
                    var team = new Team();
                    team.AddPlayer(mem.UserName, mem.ConnectionId);
                    return team;
                });

                var gameConfig = new GameRoomConfiguration
                {
                    TeamsCount = teams.Count(),
                    TeamType = TeamType.Solo,
                    TimerDifficulty = friendParty.TimerDifficulty,
                    TimerType = friendParty.TimerType,
                };

                string groupName = Guid.NewGuid().ToString();
                var gameState = this.gameStateFactory.CreateGameState(gameConfig, teams, groupName);

                partyIdsByInvitationCodes.TryRemove(new(party.InvitationCode, partyId));
                partiesByPartyIds.TryRemove(new(partyId, party));

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
            else if (party is DuoParty)
            {
                var gameMode = GameMode.Duo;
                var waitingTeam = new WaitingTeam(party.Members);
                this.AddToWaitingQueue(waitingTeam, gameMode, out hasGameStarted);
            }
        }

        public void LeaveParty(string leaverUserName, string partyId, out bool shouldDisposeParty)
        {
            Party party = this.GetPartyById(partyId);

            party.RemoveMember(leaverUserName);

            shouldDisposeParty = party.IsEmpty;
        }

        public void DisposeParty(string partyId)
        {
            Party party = this.GetPartyById(partyId);
            partyIdsByInvitationCodes.TryRemove(new(party.InvitationCode, party.Id));
            partiesByPartyIds.TryRemove(new(party.Id, party));
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

        public Party GetPartyById(string partyId)
        {
            if (!partiesByPartyIds.ContainsKey(partyId))
            {
                throw new PartyNotFoundException();
            }

            return partiesByPartyIds[partyId];
        }

        private void AddToWaitingQueue(
            WaitingTeam waitingTeam, GameMode gameMode, out bool hasGameStarted)
        {
            if (!waitingTeamsByGameModes.ContainsKey(gameMode))
            {
                waitingTeamsByGameModes.TryAdd(gameMode, new());
            }

            var waitingTeams = waitingTeamsByGameModes[gameMode];
            waitingTeams.Add(waitingTeam);

            bool isQueueFull = gameMode.GetTeamsCount() > waitingTeams.Count;

            if (!isQueueFull)
            {
                hasGameStarted = false;
                return;
            }

            string groupName = Guid.NewGuid().ToString();
            var gameState = this.gameStateFactory.CreateGameState(gameMode, waitingTeams, groupName);

            waitingTeams.Clear();
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

        public Party GetPartyByInvitationCode(string invitationCode)
        {
            ThrowIfInvitationCodeIsNotExisting(invitationCode);

            string partyId = partyIdsByInvitationCodes[invitationCode];

            if (!partiesByPartyIds.ContainsKey(partyId))
            {
                throw new ArgumentException($"Party with such {partyId} was not found.");
            }

            return partiesByPartyIds[partyId];
        }

        private static void ThrowIfInvitationCodeIsNotExisting(string invitationCode)
        {
            bool isInvitationCodeExisting = partyIdsByInvitationCodes.ContainsKey(invitationCode);

            if (!isInvitationCodeExisting)
            {
                throw new UnexistingInvitationCodeException();
            }
        }
    }
}

using System.Collections.Concurrent;
using SuperScrabble.Common.Exceptions.Matchmaking;
using SuperScrabble.Common.Exceptions.Matchmaking.Party;

using SuperScrabble.Services.Common;
using SuperScrabble.Services.Game.Common;
using SuperScrabble.Services.Game.Common.Enums;
using SuperScrabble.Services.Game.Factories;
using SuperScrabble.Services.Game.Models;
using SuperScrabble.Services.Game.Models.Parties;

namespace SuperScrabble.Services.Game.Matchmaking;

public class MatchmakingService : IMatchmakingService
{
    public const string RandomDuoInvitationCode = "RANDDUO";

    private static readonly ConcurrentDictionary<string, Party> partiesByPartyIds = new();
    private static readonly ConcurrentDictionary<string, string> partyIdsByInvitationCodes = new();
    private static readonly ConcurrentDictionary<string, string> gameIdsByUserNames = new();
    private static readonly ConcurrentDictionary<string, GameState> gameStatesByGameIds = new();
    private static readonly ConcurrentDictionary<GameMode, List<WaitingTeam>> waitingTeamsByGameModes = new();

    private readonly IGameStateFactory _gameStateFactory;
    private readonly IInvitationCodeGenerator _invitationCodeGenerator;

    public MatchmakingService(
        IGameStateFactory gameStateFactory,
        IInvitationCodeGenerator invitationCodeGenerator)
    {
        _gameStateFactory = gameStateFactory;
        _invitationCodeGenerator = invitationCodeGenerator;
    }

    public string CreateParty(string creatorUserName,
        string creatorConnectionId, PartyType partyType)
    {
        while (true)
        {
            string invitationCode = _invitationCodeGenerator.GenerateInvitationCode();

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
        if (invitationCode == RandomDuoInvitationCode)
        {
            throw new UnexistingInvitationCodeException();
        }

        ThrowIfInvitationCodeIsNotExisting(invitationCode);

        Party party = GetPartyByInvitationCode(invitationCode);
        party.AddMember(new Member(joinerUserName, joinerConnectionId));

        hasEnoughPlayersToStartGame = party.HasEnoughPlayersToStartGame;
    }

    public void StartGameFromParty(string starterUserName, string partyId, out bool hasGameStarted)
    {
        hasGameStarted = false;

        Party party = GetPartyById(partyId);

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

            string gameId = Guid.NewGuid().ToString();
            var gameState = _gameStateFactory.CreateGameState(gameConfig, teams, gameId);

            partyIdsByInvitationCodes.TryRemove(new(party.InvitationCode, partyId));
            partiesByPartyIds.TryRemove(new(partyId, party));

            gameStatesByGameIds.TryAdd(gameId, gameState);

            foreach (Player player in gameState.Players)
            {
                gameIdsByUserNames.TryAdd(player.UserName, gameId);
            }

            hasGameStarted = true;
        }
        else if (party is DuoParty)
        {
            var gameMode = GameMode.Duo;
            var waitingTeam = new WaitingTeam(party.Members);
            AddToWaitingQueue(waitingTeam, gameMode, out hasGameStarted);

            partyIdsByInvitationCodes.TryRemove(new(party.InvitationCode, partyId));
            partiesByPartyIds.TryRemove(new(partyId, party));
        }
    }

    public void LeaveParty(string leaverUserName, string partyId, out bool shouldDisposeParty)
    {
        Party party = GetPartyById(partyId);
        party.RemoveMember(leaverUserName);
        shouldDisposeParty = party.IsEmpty;
    }

    public void DisposeParty(string partyId)
    {
        Party party = GetPartyById(partyId);
        partyIdsByInvitationCodes.TryRemove(new(party.InvitationCode, party.Id));
        partiesByPartyIds.TryRemove(new(party.Id, party));
    }

    public GameState GetGameState(string userName)
    {
        if (!gameIdsByUserNames.ContainsKey(userName))
        {
            throw new ArgumentException(
                $"No {nameof(GameState)} for the given {nameof(userName)} was found.");
        }

        string gameId = gameIdsByUserNames[userName];
        return gameStatesByGameIds[gameId];
    }

    public Party GetPartyById(string partyId)
    {
        if (!partiesByPartyIds.ContainsKey(partyId))
        {
            throw new PartyNotFoundException();
        }

        return partiesByPartyIds[partyId];
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

    private void AddToWaitingQueue(
        WaitingTeam waitingTeam, GameMode gameMode, out bool hasGameStarted)
    {
        if (!waitingTeamsByGameModes.ContainsKey(gameMode))
        {
            waitingTeamsByGameModes.TryAdd(gameMode, new());
        }

        var waitingTeams = waitingTeamsByGameModes[gameMode];
        waitingTeams.Add(waitingTeam);
        bool isQueueFull = gameMode.GetTeamsCount() <= waitingTeams.Count;

        if (!isQueueFull)
        {
            hasGameStarted = false;
            return;
        }

        string gameId = Guid.NewGuid().ToString();

        GameState gameState = _gameStateFactory
            .CreateGameState(gameMode, waitingTeams, gameId);

        waitingTeams.Clear();
        gameStatesByGameIds.TryAdd(gameId, gameState);

        foreach (Player player in gameState.Players)
        {
            gameIdsByUserNames.TryAdd(player.UserName, gameId);
        }

        hasGameStarted = true;
    }

    private static void ThrowIfInvitationCodeIsNotExisting(string invitationCode)
    {
        bool isInvitationCodeExisting = partyIdsByInvitationCodes.ContainsKey(invitationCode);

        if (!isInvitationCodeExisting)
        {
            throw new UnexistingInvitationCodeException();
        }
    }

    public void JoinRoom(
        string joinerUserName, string joinerConnectionId,
        GameMode gameMode, out bool hasGameStarted)
    {
        var joiner = new Member(joinerUserName, joinerConnectionId);

        AddToWaitingQueue(new WaitingTeam(new[] { joiner }), gameMode, out hasGameStarted);
    }

    public void JoinRandomDuoParty(
        string joinerUserName, string joinerConnectionId, out bool hasGameStarted)
    {
        Party party;

        try
        {
            party = GetPartyByInvitationCode(RandomDuoInvitationCode);
            party.AddMember(new Member(joinerUserName, joinerConnectionId));
        }
        catch (MatchmakingFailedException)
        {
            string partyId = CreateParty(
                joinerUserName, joinerConnectionId, PartyType.Duo);

            party = GetPartyById(partyId);
            partyIdsByInvitationCodes.TryRemove(new(party.InvitationCode, party.Id));
            partyIdsByInvitationCodes.TryAdd(RandomDuoInvitationCode, partyId);
            party.InvitationCode = RandomDuoInvitationCode;
        }

        if (!party.IsFull)
        {
            hasGameStarted = false;
            return;
        }

        StartGameFromParty(party.Owner?.UserName!, party.Id, out hasGameStarted);
    }

    public bool IsUserInsideAnyGame(string userName)
    {
        return gameIdsByUserNames.ContainsKey(userName);
    }

    public bool IsUserInsideGame(string userName, string gameId)
    {
        if (!gameIdsByUserNames.ContainsKey(userName))
        {
            return false;
        }

        return gameIdsByUserNames[userName] == gameId;
    }

    public void RemoveUserFromGame(string userName)
    {
        if (!gameIdsByUserNames.ContainsKey(userName))
        {
            return;
        }

        string gameId = gameIdsByUserNames[userName];
        gameIdsByUserNames.TryRemove(new(userName, gameId));
    }

    public void RemoveGameState(string gameId)
    {
        if (!gameStatesByGameIds.ContainsKey(gameId))
        {
            return;
        }

        var gameState = gameStatesByGameIds[gameId];
        gameStatesByGameIds.TryRemove(new(gameId, gameState));
    }
}


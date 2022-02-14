using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using SuperScrabble.Common.Exceptions.Matchmaking;

using SuperScrabble.Services.Game;
using SuperScrabble.Services.Game.Common.Enums;
using SuperScrabble.Services.Game.Common.TilesProviders;
using SuperScrabble.Services.Game.Matchmaking;
using SuperScrabble.Services.Game.Models;
using SuperScrabble.Services.Game.Models.Parties;

using SuperScrabble.WebApi.HubClients;
using SuperScrabble.WebApi.ViewModels.Game;
using SuperScrabble.WebApi.ViewModels.Party;

namespace SuperScrabble.WebApi.Hubs;

[Authorize]
public class GameHub : Hub<IGameClient>
{
    private readonly IMatchmakingService _matchmakingService;
    private readonly IGameService _gameService;
    private readonly ITilesProvider _tilesProvider;

    public GameHub(
        IMatchmakingService matchmakingService,
        IGameService gameService,
        ITilesProvider tilesProvider)
    {
        _matchmakingService = matchmakingService;
        _gameService = gameService;
        _tilesProvider = tilesProvider;
    }

    public string? UserName => Context.User?.Identity?.Name;

    public string ConnectionId => Context.ConnectionId;

    public override async Task OnConnectedAsync()
    {
        if (_matchmakingService.IsUserInsideAnyGame(UserName!))
        {
            var gameState = _matchmakingService.GetGameState(UserName!);
            var player = gameState.GetPlayer(UserName!)!;

            if (player.ConnectionId == null)
            {
                player.ConnectionId = ConnectionId;
            }

            await Clients.Caller.UserAlreadyInsideGame(gameState.GameId);
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (_matchmakingService.IsUserInsideAnyGame(UserName!))
        {
            var gameState = _matchmakingService.GetGameState(UserName!);
            var player = gameState.GetPlayer(UserName!)!;

            if (player.ConnectionId == ConnectionId)
            {
                player.ConnectionId = null;
            }
        }

        return Task.CompletedTask;
    }

    public async Task WriteWord(WriteWordInputModel input)
    {
        var gameState = _matchmakingService.GetGameState(UserName!);
        var result = _gameService.WriteWord(gameState, input, UserName!);

        if (!result.IsSucceeded)
        {
            await Clients.Caller.InvalidWriteWordInput(result);
            return;
        }

        await UpdateGameStateAsync(gameState);
    }

    public async Task LeaveGame()
    {
        var gameState = _matchmakingService.GetGameState(UserName!);

        if (gameState == null)
        {
            return;
        }

        gameState.GetPlayer(UserName!)!.LeaveGame();
        gameState.EndGameIfRoomIsEmpty();
        gameState.CurrentTeam.NextPlayer();

        if (gameState.CurrentTeam.IsTurnFinished)
        {
            gameState.NextTeam();
        }

        await UpdateGameStateAsync(gameState);
    }

    public async Task ExchangeTiles(ExchangeTilesInputModel input)
    {
        var gameState = _matchmakingService.GetGameState(UserName!);
        var result = _gameService.ExchangeTiles(gameState, input, UserName!);

        if (!result.IsSucceeded)
        {
            await Clients.Caller.InvalidExchangeTilesInput(result);
            return;
        }

        await UpdateGameStateAsync(gameState);
    }

    public async Task SkipTurn()
    {
        var gameState = _matchmakingService.GetGameState(UserName!);
        var result = _gameService.SkipTurn(gameState, UserName!);

        if (!result.IsSucceeded)
        {
            await Clients.Caller.ImpossibleToSkipTurn(result);
            return;
        }

        await UpdateGameStateAsync(gameState);
    }

    public async Task GetAllWildcardOptions()
    {
        var options = _tilesProvider.GetAllWildcardOptions();
        await Clients.Caller.ReceiveAllWildcardOptions(options);
    }

    public async Task JoinRoom(GameMode gameMode)
    {
        try
        {
            _matchmakingService.JoinRoom(
                UserName!, ConnectionId, gameMode, out bool hasGameStarted);

            if (hasGameStarted)
            {
                await StartGameAsync();
            }
        }
        catch (MatchmakingFailedException ex)
        {
            await SendErrorAsync(ex.ErrorCode);
        }
    }

    public async Task JoinRandomDuo()
    {
        _matchmakingService.JoinRandomDuoParty(
            UserName!, ConnectionId, out bool hasGameStarted);

        if (hasGameStarted)
        {
            await StartGameAsync();
        }
    }

    public async Task CreateParty(PartyType partyType)
    {
        try
        {
            string partyId = _matchmakingService
                .CreateParty(UserName!, ConnectionId, partyType);

            await Clients.Caller.PartyCreated(partyId);
        }
        catch (MatchmakingFailedException ex)
        {
            await SendErrorAsync(ex.ErrorCode);
        }
    }

    public async Task LoadParty(string partyId)
    {
        Party party = default!;

        try
        {
            party = _matchmakingService.GetPartyById(partyId);
        }
        catch (MatchmakingFailedException ex)
        {
            await SendErrorAsync(ex.ErrorCode);
            return;
        }

        const TimerType defaultTimerType = TimerType.Standard;

        var viewModel = new FriendPartyViewModel
        {
            Owner = party.Owner!.UserName,
            InvitationCode = party.InvitationCode,
            IsOwner = party.Owner?.UserName == UserName,
            Members = party.Members.Select(mem => mem.UserName),
            PartyType = party is FriendParty ? PartyType.Friendly : PartyType.Duo,
            ConfigSettings = new ConfigSetting[]
            {
                    CreateTimerTypeConfigSetting(defaultTimerType),

                    CreateTimerDifficultyConfigSetting(
                        TimerType.Standard, TimerDifficulty.Normal),
            }
        };

        await Clients.Caller.ReceivePartyData(viewModel);
    }

    public async Task JoinParty(string invitationCode)
    {
        try
        {
            _matchmakingService.JoinParty(UserName!, ConnectionId,
                invitationCode, out bool hasEnoughPlayersToStartGame);

            Party party = _matchmakingService.GetPartyByInvitationCode(invitationCode);

            if (hasEnoughPlayersToStartGame)
            {
                await Clients.Client(party.Owner!.ConnectionId).EnablePartyStart();
            }

            await Clients.Caller.PartyJoined(party.Id);

            await Clients.Clients(party.GetConnectionIds(UserName!))
                .NewPlayerJoinedParty(UserName!);
        }
        catch (PlayerAlreadyInsidePartyException)
        {
            Party party = _matchmakingService.GetPartyByInvitationCode(invitationCode);
            Member? member = party.GetMember(UserName!);

            if (member != null)
            {
                await Clients.Client(member.ConnectionId).PartyMemberConnectionIdChanged();
                member.ConnectionId = ConnectionId;
            }

            await Clients.Caller.PartyJoined(party.Id);
        }
        catch (MatchmakingFailedException ex)
        {
            await SendErrorAsync(ex.ErrorCode);
        }
    }

    public async Task LeaveParty(string partyId)
    {
        try
        {
            _matchmakingService.LeaveParty(
                UserName!, partyId, out bool shouldDisposeParty);

            await Clients.Caller.PartyLeft();

            if (!shouldDisposeParty)
            {
                Party party = _matchmakingService.GetPartyById(partyId);

                foreach (Member member in party.Members)
                {
                    var viewModel = new PlayerHasLeftPartyViewModel
                    {
                        Owner = party.Owner?.UserName!,
                        IsOwner = party.Owner?.UserName == member.UserName,
                        RemainingMembers = party.Members.Select(mem => mem.UserName),
                        LeaverUserName = UserName!,
                        IsPartyReady = party.HasEnoughPlayersToStartGame,
                    };

                    await Clients.Client(member.ConnectionId).PlayerHasLeftParty(viewModel);
                }
            }

            if (shouldDisposeParty)
            {
                _matchmakingService.DisposeParty(partyId);
            }
        }
        catch (MatchmakingFailedException ex)
        {
            await SendErrorAsync(ex.ErrorCode);
        }
    }

    public async Task StartGameFromParty(string partyId)
    {
        try
        {
            Party party = _matchmakingService.GetPartyById(partyId);

            _matchmakingService.StartGameFromParty(
                UserName!, partyId, out bool hasGameStarted);

            if (!hasGameStarted)
            {
                // Duo Game with friend
                // TODO: Redirect to loading screen
                return;
            }

            await StartGameAsync();
        }
        catch (MatchmakingFailedException ex)
        {
            await SendErrorAsync(ex.ErrorCode);
        }
    }

    public async Task SetFriendPartyConfiguration(FriendPartyConfig config, string partyId)
    {
        try
        {
            Party party = _matchmakingService.GetPartyById(partyId);

            if (party.Owner?.UserName != UserName)
            {
                throw new OnlyOwnerHasAccessException();
            }

            if (party is FriendParty friendParty)
            {
                friendParty.TimerType = config.TimerType;
                friendParty.TimerDifficulty = config.TimerDifficulty;

                var configSettings = new ConfigSetting[]
                {
                        CreateTimerTypeConfigSetting(friendParty.TimerType),

                        CreateTimerDifficultyConfigSetting(
                            friendParty.TimerType, friendParty.TimerDifficulty),
                };

                await Clients
                    .Clients(friendParty.GetConnectionIds())
                    .UpdateFriendPartyConfigSettings(configSettings);
            }
        }
        catch (MatchmakingFailedException ex)
        {
            await SendErrorAsync(ex.ErrorCode);
        }
    }

    public async Task LoadGame(string gameId)
    {
        if (!_matchmakingService.IsUserInsideGame(UserName!, gameId))
        {
            // User is NOT inside the given game
            return;
        }

        var gameState = _matchmakingService.GetGameState(UserName!);
        var player = gameState.GetPlayer(UserName!)!;

        if (player.ConnectionId != ConnectionId)
        {
            await Clients.Client(player.ConnectionId!).UserEnteredGameFromAnotherConnectionId();
            player.ConnectionId = ConnectionId;
        }

        var viewModel = _gameService.MapFromGameState(gameState, UserName!);
        await Clients.Client(ConnectionId).UpdateGameState(viewModel);
    }

    private async Task SendErrorAsync(string message)
    {
        await Clients.Caller.Error(message);
    }

    private async Task StartGameAsync()
    {
        var gameState = _matchmakingService.GetGameState(UserName!);
        string gameId = gameState.GameId;

        foreach (Player player in gameState.Players)
        {
            await Groups.AddToGroupAsync(player.ConnectionId!, gameId);
        }

        await Clients.Group(gameId).StartGame(gameId);
        await UpdateGameStateAsync(gameState);
    }

    private async Task UpdateGameStateAsync(GameState gameState)
    {
        foreach (Player player in gameState.Players)
        {
            _gameService.FillPlayerTiles(gameState, player);
        }

        string gameId = gameState.GameId;

        foreach (Player player in gameState.Players)
        {
            var viewModel = _gameService.MapFromGameState(gameState, player.UserName);
            await Clients.Client(player.ConnectionId!).UpdateGameState(viewModel);

            if (gameState.IsGameOver)
            {
                _matchmakingService.RemoveUserFromGame(player.UserName);
                await Groups.RemoveFromGroupAsync(player.ConnectionId!, gameId);
            }
        }

        if (gameState.IsGameOver)
        {
            _matchmakingService.RemoveGameState(gameId);
        }
    }

    private static ConfigSetting CreateTimerTypeConfigSetting(TimerType selectedTimerType)
    {
        return new ConfigSetting
        {
            Name = nameof(TimerType),
            Options = Enum
                .GetValues(typeof(TimerType)).Cast<TimerType>()
                .Select(value => new SettingOption
                {
                    Value = (int)value,
                    Name = value.ToString(),
                    IsSelected = value == selectedTimerType,
                })
        };
    }

    private static ConfigSetting CreateTimerDifficultyConfigSetting(
        TimerType selectedTimerType, TimerDifficulty selectedTimerDifficulty)
    {
        return new ConfigSetting
        {
            Name = nameof(TimerDifficulty),
            Options = Enum
                .GetValues(typeof(TimerDifficulty)).Cast<TimerDifficulty>()
                .Select(value => new SettingOption
                {
                    Value = (int)value,
                    IsSelected = value == selectedTimerDifficulty,
                    Name = TimeSpan.FromSeconds(
                        value.GetSeconds(selectedTimerType)).ToString("mm':'ss"),
                })
        };
    }
}


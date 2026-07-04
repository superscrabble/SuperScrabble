using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using SuperScrabble.Common.Exceptions.Matchmaking;
using SuperScrabble.Services.Data.Games;
using SuperScrabble.Services.Game;
using SuperScrabble.Services.Game.Common;
using SuperScrabble.Services.Game.Common.Enums;
using SuperScrabble.Services.Game.Common.TilesProviders;
using SuperScrabble.Services.Game.Matchmaking;
using SuperScrabble.Services.Game.Models;
using SuperScrabble.Services.Game.Models.Parties;

using SuperScrabble.WebApi.HubClients;
using SuperScrabble.WebApi.Timers;
using SuperScrabble.WebApi.ViewModels.Games;
using SuperScrabble.WebApi.ViewModels.Party;

namespace SuperScrabble.WebApi.Hubs;

[Authorize]
public class GameHub : Hub<IGameClient>
{
    public const string UserNotInsideAGameErrorCode = "UserNotInsideAGame";

    // The waiting queues and parties are global state, so matchmaking operations are
    // serialized; game commands are serialized per game via GameLocks (shared with timers).
    // Static because hubs are instantiated per invocation.
    private static readonly SemaphoreSlim MatchmakingLock = new(1, 1);

    private readonly IMatchmakingService _matchmakingService;
    private readonly IGameService _gameService;
    private readonly ITilesProvider _tilesProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimerManager _timerManager;
    private readonly IGamesService _gamesService;

    public GameHub(
        IMatchmakingService matchmakingService,
        IGameService gameService,
        ITilesProvider tilesProvider,
        IServiceProvider serviceProvider,
        TimerManager timerManager,
        IGamesService gamesService)
    {
        _matchmakingService = matchmakingService;
        _gameService = gameService;
        _tilesProvider = tilesProvider;
        _serviceProvider = serviceProvider;
        _timerManager = timerManager;
        _gamesService = gamesService;
    }

    public string UserName => Context.User?.Identity?.Name!;

    public string ConnectionId => Context.ConnectionId;

    public override async Task OnConnectedAsync()
    {
        if (_matchmakingService.IsUserInsideAnyGame(UserName))
        {
            var gameState = _matchmakingService.GetGameState(UserName);
            var player = gameState.GetPlayer(UserName)!;

            if (player.ConnectionId == null)
            {
                player.ConnectionId = ConnectionId;
            }

            await Clients.Caller.UserAlreadyInsideGame(new
            {
                GameId = gameState.GameId,
                GameMode = (int)gameState.GameMode,
            });
        }
        else if (_matchmakingService.IsUserInsideAnyParty(UserName))
        {
            var party = _matchmakingService.GetPartyByUserName(UserName)!;
            var member = party.GetMember(UserName)!;
            member.ConnectionId = ConnectionId;
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (_matchmakingService.IsUserInsideAnyGame(UserName))
        {
            var gameState = _matchmakingService.GetGameState(UserName);
            var player = gameState.GetPlayer(UserName)!;

            if (player.ConnectionId == ConnectionId)
            {
                player.ConnectionId = null;
            }
        }
        else if (_matchmakingService.IsUserInsideAnyParty(UserName))
        {
            var party = _matchmakingService.GetPartyByUserName(UserName)!;
            var member = party.GetMember(UserName)!;
            member.ConnectionId = null;
        }

        return Task.CompletedTask;
    }

    public async Task WriteWord(WriteWordInputModel input)
    {
        await ExecuteGameActionAsync(async gameState =>
        {
            var result = _gameService.WriteWord(gameState, input, UserName);

            if (!result.IsSucceeded)
            {
                await Clients.Caller.InvalidWriteWordInput(result);
                return;
            }

            await SaveGameIfTheGameIsOverAsync(gameState);
            await UpdateGameStateAsync(gameState);
        });
    }

    public async Task LeaveGame()
    {
        await ExecuteGameActionAsync(async gameState =>
        {
            gameState.GetPlayer(UserName)!.LeaveGame();
            gameState.AddHistoryLog(new GameHistoryLog
            {
                Status = HistoryLogStatus.Leave,
                UserName = UserName,
            });

            gameState.EndGameIfRoomIsEmptyOrAllPlayersHaveRunOutOfTime();
            gameState.CurrentTeam.NextPlayer();

            if (gameState.CurrentTeam.IsTurnFinished)
            {
                gameState.NextTeam();
            }

            await SaveGameIfTheGameIsOverAsync(gameState);
            await UpdateGameStateAsync(gameState);
        },
        notifyWhenNotInsideAGame: false);
    }

    public async Task ExchangeTiles(ExchangeTilesInputModel input)
    {
        await ExecuteGameActionAsync(async gameState =>
        {
            var result = _gameService.ExchangeTiles(gameState, input, UserName);

            if (!result.IsSucceeded)
            {
                await Clients.Caller.InvalidExchangeTilesInput(result);
                return;
            }

            await UpdateGameStateAsync(gameState);
        });
    }

    public async Task SkipTurn()
    {
        await ExecuteGameActionAsync(async gameState =>
        {
            var result = _gameService.SkipTurn(gameState, UserName);

            if (!result.IsSucceeded)
            {
                await Clients.Caller.ImpossibleToSkipTurn(result);
                return;
            }

            await SaveGameIfTheGameIsOverAsync(gameState);
            await UpdateGameStateAsync(gameState);
        });
    }

    private async Task ExecuteGameActionAsync(
        Func<GameState, Task> gameAction, bool notifyWhenNotInsideAGame = true)
    {
        if (!_matchmakingService.IsUserInsideAnyGame(UserName))
        {
            if (notifyWhenNotInsideAGame)
            {
                await Clients.Caller.Error(UserNotInsideAGameErrorCode);
            }

            return;
        }

        GameState gameState = _matchmakingService.GetGameState(UserName);
        SemaphoreSlim gameLock = GameLocks.Get(gameState.GameId);
        await gameLock.WaitAsync();

        try
        {
            // Re-check after acquiring the lock: the game may have ended meanwhile.
            if (!_matchmakingService.GameExists(gameState.GameId))
            {
                if (notifyWhenNotInsideAGame)
                {
                    await Clients.Caller.Error(UserNotInsideAGameErrorCode);
                }

                return;
            }

            await gameAction(gameState);
        }
        finally
        {
            gameLock.Release();
        }
    }

    public async Task GetAllWildcardOptions()
    {
        var options = _tilesProvider.GetAllWildcardOptions();
        await Clients.Caller.ReceiveAllWildcardOptions(options);
    }

    public async Task JoinRoom(GameMode gameMode)
    {
        await MatchmakingLock.WaitAsync();

        try
        {
            if (_matchmakingService.IsUserInsideAnyGame(UserName))
            {
                return;
            }

            if (_matchmakingService.IsUserInsideAnyParty(UserName))
            {
                return;
            }

            await StopSearchingCore();

            _matchmakingService.JoinRoom(
                UserName, ConnectionId, gameMode, out bool hasGameStarted);

            if (hasGameStarted)
            {
                await StartGameAsync();
            }
            else
            {
                await Clients.Caller.WaitingQueueJoined();
            }
        }
        catch (MatchmakingFailedException ex)
        {
            await SendErrorAsync(ex.ErrorCode);
        }
        finally
        {
            MatchmakingLock.Release();
        }
    }

    public async Task StopSearching()
    {
        await MatchmakingLock.WaitAsync();

        try
        {
            await StopSearchingCore();
        }
        finally
        {
            MatchmakingLock.Release();
        }
    }

    private async Task StopSearchingCore()
    {
        var connectionIds = _matchmakingService.LeaveRoom(UserName);
        await Clients.Clients(connectionIds).SearchingStopped();
    }

    public async Task JoinRandomDuo()
    {
        await MatchmakingLock.WaitAsync();

        try
        {
            if (_matchmakingService.IsUserInsideAnyGame(UserName))
            {
                return;
            }

            if (_matchmakingService.IsUserInsideAnyParty(UserName))
            {
                return;
            }

            _matchmakingService.JoinRandomDuoParty(
                UserName, ConnectionId, out bool hasGameStarted);

            if (hasGameStarted)
            {
                await StartGameAsync();
            }
        }
        catch (MatchmakingFailedException ex)
        {
            await SendErrorAsync(ex.ErrorCode);
        }
        finally
        {
            MatchmakingLock.Release();
        }
    }

    public async Task CreateParty(PartyType partyType)
    {
        await MatchmakingLock.WaitAsync();

        try
        {
            if (_matchmakingService.IsUserInsideAnyGame(UserName))
            {
                return;
            }

            if (_matchmakingService.IsUserInsideAnyParty(UserName))
            {
                return;
            }

            string partyId = _matchmakingService
                .CreateParty(UserName, ConnectionId, partyType);

            await Clients.Caller.PartyCreated(partyId);
        }
        catch (MatchmakingFailedException ex)
        {
            await SendErrorAsync(ex.ErrorCode);
        }
        finally
        {
            MatchmakingLock.Release();
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

        TimerType selectedTimerType = TimerType.Standard;
        TimerDifficulty selectedTimerDifficulty = TimerDifficulty.Normal;

        if (party is FriendParty friendParty)
        {
            selectedTimerType = friendParty.TimerType;
            selectedTimerDifficulty = friendParty.TimerDifficulty;
        }

        var viewModel = new FriendPartyViewModel
        {
            Owner = party.Owner!.UserName,
            InvitationCode = party.InvitationCode,
            IsOwner = party.Owner?.UserName == UserName,
            Members = party.Members.Select(mem => mem.UserName),
            PartyType = party is FriendParty ? PartyType.Friendly : PartyType.Duo,
            IsPartyReady = party.HasEnoughPlayersToStartGame,
            ConfigSettings = new ConfigSetting[]
            {
                CreateTimerTypeConfigSetting(selectedTimerType),

                CreateTimerDifficultyConfigSetting(
                    selectedTimerType, selectedTimerDifficulty),
            }
        };

        await Clients.Caller.ReceivePartyData(viewModel);
    }

    public async Task JoinParty(string invitationCode)
    {
        await MatchmakingLock.WaitAsync();

        try
        {
            _matchmakingService.JoinParty(UserName, ConnectionId,
                invitationCode, out bool hasEnoughPlayersToStartGame);

            Party party = _matchmakingService.GetPartyByInvitationCode(invitationCode);

            if (hasEnoughPlayersToStartGame)
            {
                await Clients.Client(party.Owner!.ConnectionId!).EnablePartyStart();
            }

            await Clients.Caller.PartyJoined(party.Id);

            await Clients.Clients(party.GetConnectionIds(UserName))
                .NewPlayerJoinedParty(UserName);
        }
        catch (PlayerAlreadyInsidePartyException)
        {
            Party party = _matchmakingService.GetPartyByInvitationCode(invitationCode);
            Member? member = party.GetMember(UserName);

            if (member != null)
            {
                await Clients.Client(member.ConnectionId!).PartyMemberConnectionIdChanged();
                member.ConnectionId = ConnectionId;
            }

            await Clients.Caller.PartyJoined(party.Id);
        }
        catch (MatchmakingFailedException ex)
        {
            await SendErrorAsync(ex.ErrorCode);
        }
        finally
        {
            MatchmakingLock.Release();
        }
    }

    public async Task LeaveParty(string partyId)
    {
        await MatchmakingLock.WaitAsync();

        try
        {
            _matchmakingService.LeaveParty(
                UserName, partyId, out bool shouldDisposeParty);

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
                        LeaverUserName = UserName,
                        IsPartyReady = party.HasEnoughPlayersToStartGame,
                    };

                    await Clients.Client(member.ConnectionId!).PlayerHasLeftParty(viewModel);
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
        finally
        {
            MatchmakingLock.Release();
        }
    }

    public async Task StartGameFromParty(string partyId)
    {
        await MatchmakingLock.WaitAsync();

        try
        {
            Party party = _matchmakingService.GetPartyById(partyId);

            _matchmakingService.StartGameFromParty(
                UserName, partyId, out bool hasGameStarted);

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
        finally
        {
            MatchmakingLock.Release();
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
        if (!_matchmakingService.GameExists(gameId))
        {
            await Clients.Client(this.ConnectionId).NoSuchGame();
            return;
        }

        if (!_matchmakingService.IsUserInsideGame(UserName, gameId))
        {
            //TODO: add and remove spectators
            return;
        }

        var gameState = _matchmakingService.GetGameState(UserName);
        var player = gameState.GetPlayer(UserName)!;

        if (player.ConnectionId != ConnectionId)
        {
            await Clients.Client(player.ConnectionId!).UserEnteredGameFromAnotherConnectionId();
            player.ConnectionId = ConnectionId;
        }

        var viewModel = _gameService.MapFromGameState(gameState, UserName);
        await Clients.Client(player.ConnectionId).UpdateGameState(viewModel);
    }

    private async Task SaveGameIfTheGameIsOverAsync(GameState gameState)
    {
        if (gameState.IsGameOver)
        {
            var saveGameInput = new SaveGameInputModel
            {
                Players = gameState.Players,
                GameId = gameState.GameId
            };

            _timerManager.GetTimer(gameState.GameId)?.Dispose();
            await _gamesService.SaveGameAsync(saveGameInput);
        }
    }

    private async Task SendErrorAsync(string message)
    {
        await Clients.Caller.Error(message);
    }

    private async Task StartGameAsync()
    {
        var gameState = _matchmakingService.GetGameState(UserName);
        string gameId = gameState.GameId;

        foreach (Player player in gameState.Players)
        {
            await Groups.AddToGroupAsync(player.ConnectionId!, gameId);
        }

        var timer = _timerManager.CreateTimer(gameState);
        _timerManager.AttachTimerToGameState(timer, gameId);

        await Clients.Group(gameId).StartGame(gameId);
        await UpdateGameStateAsync(gameState);

        timer.Start();
    }

    private async Task UpdateGameStateAsync(GameState gameState)
    {
        foreach (Player player in gameState.Players)
        {
            _gameService.FillPlayerTiles(gameState, player);
        }

        string gameId = gameState.GameId;

        var timer = _timerManager.GetTimer(gameId)!;
        timer.Reset();

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
            _timerManager.RemoveTimer(gameId);
            GameLocks.Remove(gameId);
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


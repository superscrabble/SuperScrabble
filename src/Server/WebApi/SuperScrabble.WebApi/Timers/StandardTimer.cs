using Microsoft.AspNetCore.SignalR;

using SuperScrabble.Services.Data.Games;
using SuperScrabble.Services.Game;
using SuperScrabble.Services.Game.Matchmaking;
using SuperScrabble.Services.Game.Models;

using SuperScrabble.WebApi.HubClients;
using SuperScrabble.WebApi.Hubs;
using SuperScrabble.WebApi.ViewModels.Games;

namespace SuperScrabble.WebApi.Timers;

public class StandardTimer : GameTimer
{
    private readonly GameState _gameState;
    private readonly IHubContext<GameHub, IGameClient> _hubContext;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<StandardTimer> _logger;

    public StandardTimer(
        GameState gameState,
        IHubContext<GameHub, IGameClient> hubContext,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<StandardTimer> logger)
    {
        _gameState = gameState;
        _hubContext = hubContext;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        Reset();

        _timer.Elapsed += async (sender, args) => await OnTimedEventSafeAsync();
    }

    public int SecondsRemaining { get; private set; }

    public override void Reset()
    {
        SecondsRemaining = _gameState.GameplayConstants.GameTimerSeconds;
        base.Reset();
    }

    private async Task OnTimedEventSafeAsync()
    {
        // Timer callbacks run on threadpool threads concurrently with hub invocations,
        // so game-state mutation happens under the same per-game lock the hub uses.
        SemaphoreSlim gameLock = GameLocks.Get(_gameState.GameId);
        await gameLock.WaitAsync();

        try
        {
            await OnTimedEventAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StandardTimer tick failed for game {GameId}.", _gameState.GameId);
        }
        finally
        {
            gameLock.Release();
        }
    }

    private async Task OnTimedEventAsync()
    {
        if (_gameState.IsGameOver)
        {
            return;
        }

        if (SecondsRemaining >= 0)
        {
            var viewModel = new UpdateGameTimerViewModel
            {
                Minutes = SecondsRemaining / 60,
                Seconds = SecondsRemaining % 60,
            };

            foreach (Player player in _gameState.Players)
            {
                if (player.ConnectionId == null)
                {
                    continue;
                }

                await _hubContext.Clients
                    .Client(player.ConnectionId)
                    .UpdateGameTimer(viewModel);
            }

            SecondsRemaining--;
            return;
        }

        // AFK procedure
        _gameState.CurrentTeam.CurrentPlayer.ConsecutiveSkipsCount++;

        _gameState.NextTeam();

        // Scoped services must come from a fresh scope: the timer outlives the hub
        // invocation that created it.
        using var scope = _serviceScopeFactory.CreateScope();
        var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
        var matchmakingService = scope.ServiceProvider.GetRequiredService<IMatchmakingService>();

        foreach (Player player in _gameState.Players)
        {
            gameService.FillPlayerTiles(_gameState, player);
        }

        foreach (Player player in _gameState.Players)
        {
            if (player.ConnectionId == null)
            {
                continue;
            }

            var viewModel = gameService.MapFromGameState(_gameState, player.UserName);

            await _hubContext.Clients
                .Client(player.ConnectionId)
                .UpdateGameState(viewModel);

            if (_gameState.IsGameOver)
            {
                matchmakingService.RemoveUserFromGame(player.UserName);

                await _hubContext.Groups
                    .RemoveFromGroupAsync(player.ConnectionId, _gameState.GameId);
            }
        }

        if (_gameState.IsGameOver)
        {
            matchmakingService.RemoveGameState(_gameState.GameId);

            var gamesService = scope.ServiceProvider.GetRequiredService<IGamesService>();

            await gamesService.SaveGameAsync(new SaveGameInputModel
            {
                GameId = _gameState.GameId,
                Players = _gameState.Players,
            });

            GameLocks.Remove(_gameState.GameId);
            Dispose();
            return;
        }

        Reset();
    }
}

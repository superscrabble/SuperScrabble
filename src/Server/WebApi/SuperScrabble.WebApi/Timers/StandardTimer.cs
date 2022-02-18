using System.Timers;

using Microsoft.AspNetCore.SignalR;

using SuperScrabble.Services.Game;
using SuperScrabble.Services.Game.Matchmaking;
using SuperScrabble.Services.Game.Models;

using SuperScrabble.WebApi.HubClients;
using SuperScrabble.WebApi.Hubs;
using SuperScrabble.WebApi.ViewModels.Game;

namespace SuperScrabble.WebApi.Timers;

public class StandardTimer : System.Timers.Timer
{
    private const int IntervalSeconds = 1;

    private readonly GameState _gameState;
    private readonly IGameService _gameService;
    private readonly IMatchmakingService _matchmakingService;
    private readonly IHubContext<GameHub, IGameClient> _hubContext;

    public StandardTimer(
        GameState gameState,
        IGameService gameService,
        IMatchmakingService matchmakingService,
        IHubContext<GameHub, IGameClient> hubContext)
    {
        _gameState = gameState;
        _gameService = gameService;
        _matchmakingService = matchmakingService;
        _hubContext = hubContext;

        Reset();

        Elapsed += async (sender, args) => await OnTimedEvent(sender, args);
    }

    public int SecondsRemaining { get; private set; }

    public void Reset()
    {
        AutoReset = true;
        Interval = IntervalSeconds * 1_000;
        SecondsRemaining = _gameState.GameplayConstants.GameTimerSeconds;
    }

    private async Task OnTimedEvent(object? sender, ElapsedEventArgs args)
    {
        if (SecondsRemaining >= 0)
        {
            int minutes = SecondsRemaining / 60;
            int seconds = SecondsRemaining % 60;

            var viewModel = new UpdateGameTimerViewModel
            {
                Minutes = minutes,
                Seconds = seconds,
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

        _gameState.NextTeam();

        foreach (Player player in _gameState.Players)
        {
            _gameService.FillPlayerTiles(_gameState, player);
        }

        foreach (Player player in _gameState.Players)
        {
            if (player.ConnectionId == null)
            {
                continue;
            }

            var viewModel = _gameService.MapFromGameState(_gameState, player.UserName);

            await _hubContext.Clients
                .Client(player.ConnectionId)
                .UpdateGameState(viewModel);

            if (_gameState.IsGameOver)
            {
                _matchmakingService.RemoveUserFromGame(player.UserName);

                await _hubContext.Groups
                    .RemoveFromGroupAsync(
                        player.ConnectionId, _gameState.GameId);
            }
        }

        if (_gameState.IsGameOver)
        {
            _matchmakingService.RemoveGameState(_gameState.GameId);
        }

        Reset();
    }
}
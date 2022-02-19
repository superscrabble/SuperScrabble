using Microsoft.AspNetCore.SignalR;
using SuperScrabble.Services.Game;
using SuperScrabble.Services.Game.Matchmaking;
using SuperScrabble.Services.Game.Models;
using SuperScrabble.WebApi.HubClients;
using SuperScrabble.WebApi.Hubs;
using SuperScrabble.WebApi.ViewModels.Game;
using System.Timers;

namespace SuperScrabble.WebApi.Timers;

public class ChessTimer : GameTimer
{
    private readonly GameState _gameState;
    private readonly IGameService _gameService;
    private readonly IHubContext<GameHub, IGameClient> _hubContext;
    private readonly IMatchmakingService _matchmakingService;

    public ChessTimer(
        GameState gameState,
        IGameService gameService,
        IHubContext<GameHub, IGameClient> hubContext,
        IMatchmakingService matchmakingService)
    {
        _gameState = gameState;
        _gameService = gameService;
        _hubContext = hubContext;
        _matchmakingService = matchmakingService;

        Reset();

        _timer.Elapsed += async (sender, args) => await OnTimedEvent(sender, args);
    }

    public int SecondsRemaining { get; private set; }

    public override void Reset()
    {
        string currentPlayerName = _gameState.CurrentTeam.CurrentPlayer.UserName;
        SecondsRemaining = _gameState.SecondsRemainingByUserNames[currentPlayerName];
        base.Reset();
    }

    private async Task OnTimedEvent(object? sender, ElapsedEventArgs args)
    {
        if (SecondsRemaining >= 0)
        {
            string currentPlayerUserName = _gameState.CurrentTeam.CurrentPlayer.UserName;
            _gameState.SecondsRemainingByUserNames[currentPlayerUserName] = SecondsRemaining;

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

        if (_gameState.SecondsRemainingByUserNames.All(x => x.Value <= 0))
        {
            _gameState.EndGame();
        }
        else
        {
            _gameState.NextTeam();
        }

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
            _timer.Dispose();
        }

        Reset();
    }
}

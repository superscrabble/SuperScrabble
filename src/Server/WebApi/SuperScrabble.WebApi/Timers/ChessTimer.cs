using Microsoft.AspNetCore.SignalR;
using SuperScrabble.Services.Data.Games;
using SuperScrabble.Services.Game;
using SuperScrabble.Services.Game.Matchmaking;
using SuperScrabble.Services.Game.Models;
using SuperScrabble.WebApi.HubClients;
using SuperScrabble.WebApi.Hubs;
using SuperScrabble.WebApi.ViewModels.Games;
using System.Timers;

namespace SuperScrabble.WebApi.Timers;

public class ChessTimer : GameTimer
{
    private readonly GameState _gameState;
    private readonly IGameService _gameService;
    private readonly IHubContext<GameHub, IGameClient> _hubContext;
    private readonly IMatchmakingService _matchmakingService;
    private readonly IGamesService _gamesService;

    public ChessTimer(
        GameState gameState,
        IGamesService gamesService,
        IGameService gameService,
        IHubContext<GameHub, IGameClient> hubContext,
        IMatchmakingService matchmakingService)
    {
        _gameState = gameState;
        _gameService = gameService;
        _hubContext = hubContext;
        _matchmakingService = matchmakingService;
        _gamesService = gamesService;
        Reset();

        _timer.Elapsed += async (sender, args) => await OnTimedEvent(sender, args);
    }

    public int SecondsRemaining { get; private set; }

    public override void Reset()
    {
        string currentPlayerName = _gameState.CurrentTeam.CurrentPlayer.UserName;
        SecondsRemaining = _gameState.RemainingSecondsByUserNames[currentPlayerName];
        base.Reset();
    }

    // BackgroundService

    private async Task OnTimedEvent(object? sender, ElapsedEventArgs args)
    {
        if (SecondsRemaining >= 0)
        {
            string currentPlayerUserName = _gameState.CurrentTeam.CurrentPlayer.UserName;
            _gameState.RemainingSecondsByUserNames[currentPlayerUserName] = SecondsRemaining;

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

        if (_gameState.RemainingSecondsByUserNames.All(x => x.Value <= 0))
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

            // UserManager is Disposed
            // Database service
            await _gamesService!.SaveGameAsync(new SaveGameInputModel
            {
                GameId = _gameState.GameId,
                Players = _gameState.Players
            });

            Dispose();
        }

        Reset();
    }
}

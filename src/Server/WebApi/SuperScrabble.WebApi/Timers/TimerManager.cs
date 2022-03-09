using SuperScrabble.Services.Data.Games;
using SuperScrabble.Services.Game.Models;

namespace SuperScrabble.WebApi.Timers;

public class TimerManager
{
    private static readonly Dictionary<string, GameTimer> gameTimersByGameIds = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly IGamesService _gamesService;

    public TimerManager(
        IServiceProvider serviceProvider,
        IGamesService gamesService)
    {
        _serviceProvider = serviceProvider;
        _gamesService = gamesService;
    }

    public void AttachTimerToGameState(GameTimer timer, string gameId)
    {
        if (!gameTimersByGameIds.ContainsKey(gameId))
        {
            gameTimersByGameIds.Add(gameId, timer);
            return;
        }

        gameTimersByGameIds[gameId] = timer;
    }

    public GameTimer? GetTimer(string gameId)
    {
        if (!gameTimersByGameIds.ContainsKey(gameId))
        {
            return null;
        }

        return gameTimersByGameIds[gameId];
    }

    public GameTimer CreateTimer(GameState gameState)
    {
        if (gameState.RemainingSecondsByUserNames.Count == 0)
        {
            return ActivatorUtilities.CreateInstance<StandardTimer>(_serviceProvider, gameState);
        }
        else
        {
            return ActivatorUtilities.CreateInstance<ChessTimer>(_serviceProvider, gameState, _gamesService);
        }
    }
}

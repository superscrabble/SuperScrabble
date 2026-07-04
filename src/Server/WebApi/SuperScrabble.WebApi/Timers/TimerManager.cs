using System.Collections.Concurrent;

using SuperScrabble.Services.Game.Models;

namespace SuperScrabble.WebApi.Timers;

public class TimerManager
{
    private static readonly ConcurrentDictionary<string, GameTimer> gameTimersByGameIds = new();

    private readonly IServiceProvider _serviceProvider;

    public TimerManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void AttachTimerToGameState(GameTimer timer, string gameId)
    {
        gameTimersByGameIds[gameId] = timer;
    }

    public GameTimer? GetTimer(string gameId)
    {
        return gameTimersByGameIds.TryGetValue(gameId, out GameTimer? timer) ? timer : null;
    }

    public void RemoveTimer(string gameId)
    {
        if (gameTimersByGameIds.TryRemove(gameId, out GameTimer? timer))
        {
            timer.Dispose();
        }
    }

    public GameTimer CreateTimer(GameState gameState)
    {
        if (gameState.RemainingSecondsByUserNames.Count == 0)
        {
            return ActivatorUtilities.CreateInstance<StandardTimer>(_serviceProvider, gameState);
        }
        else
        {
            return ActivatorUtilities.CreateInstance<ChessTimer>(_serviceProvider, gameState);
        }
    }
}

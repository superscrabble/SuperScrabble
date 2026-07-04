using System.Collections.Concurrent;

namespace SuperScrabble.WebApi;

/// <summary>
/// One lock per running game, shared by hub invocations and game timers so that
/// a GameState is never mutated concurrently. Locks are removed when a game ends.
/// </summary>
public static class GameLocks
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> locksByGameIds = new();

    public static SemaphoreSlim Get(string gameId)
        => locksByGameIds.GetOrAdd(gameId, _ => new SemaphoreSlim(1, 1));

    public static void Remove(string gameId)
        => locksByGameIds.TryRemove(gameId, out _);
}

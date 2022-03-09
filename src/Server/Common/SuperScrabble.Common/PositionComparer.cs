using System.Diagnostics.CodeAnalysis;

namespace SuperScrabble.Common;

public class PositionComparer : IEqualityComparer<Position>
{
    public bool Equals(Position? x, Position? y)
        => x?.Row == y?.Row && x?.Column == y?.Column;

    public int GetHashCode([DisallowNull] Position position)
        => position.GetHashCode();
}

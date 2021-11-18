namespace SuperScrabble.Services
{
    using System.Collections.Generic;

    public interface IShuffleService
    {
        IEnumerable<T> Shuffle<T>(IEnumerable<T> items);
    }
}

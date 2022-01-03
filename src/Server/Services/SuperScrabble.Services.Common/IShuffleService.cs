namespace SuperScrabble.Services.Common
{
    public interface IShuffleService
    {
        IEnumerable<T> Shuffle<T>(IEnumerable<T> items);
    }
}

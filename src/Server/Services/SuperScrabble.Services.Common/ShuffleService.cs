namespace SuperScrabble.Services.Common
{
    public class ShuffleService : IShuffleService
    {
        public IEnumerable<T> Shuffle<T>(IEnumerable<T> items)
        {
            var random = new Random();
            var shuffledItems = new List<T>();
            var itemsList = items.ToList();

            while (itemsList.Count > 0)
            {
                int index = random.Next(itemsList.Count);
                shuffledItems.Add(itemsList[index]);
                itemsList.RemoveAt(index);
            }

            return shuffledItems;
        }
    }
}

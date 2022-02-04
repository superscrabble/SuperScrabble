namespace SuperScrabble.Services.Game.Common
{
    public class TileInfo
    {
        public TileInfo(int points, int count)
        {
            this.Points = points;
            this.Count = count;
        }

        public int Points { get; set; }

        public int Count { get; set; }
    }
}

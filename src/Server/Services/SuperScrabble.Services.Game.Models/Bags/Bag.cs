namespace SuperScrabble.Services.Game.Models.Bags
{
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Common.TilesProviders;

    public class Bag : IBag
    {
        private readonly List<Tile> tiles = new();

        public Bag(ITilesProvider tilesProvider)
        {
            this.InitializeTiles(tilesProvider);
        }

        public int TilesCount => this.tiles.Count;

        public Tile? DrawTile()
        {
            if (this.TilesCount <= 0)
            {
                return null;
            }

            var random = new Random();
            int index = random.Next(this.TilesCount);

            Tile tile = this.tiles[index];
            this.tiles.RemoveAt(index);

            return tile;
        }

        public IEnumerable<Tile> SwapTiles(IEnumerable<Tile> tiles)
        {
            var drawnTiles = new List<Tile>();

            for (int i = 0; i < tiles.Count(); i++)
            {
                Tile? drawnTile = this.DrawTile();

                if (drawnTile == null)
                {
                    break;
                }

                drawnTiles.Add(drawnTile);
            }

            this.tiles.AddRange(tiles);
            return drawnTiles;
        }

        private void InitializeTiles(ITilesProvider tilesProvider)
        {
            foreach (var tileInfoByLetter in tilesProvider.GetTiles())
            {
                for (int i = 0; i < tileInfoByLetter.Value.Count; i++)
                {
                    var tile = new Tile(tileInfoByLetter.Key, tileInfoByLetter.Value.Points);
                    this.tiles.Add(tile);
                }
            }
        }
    }
}

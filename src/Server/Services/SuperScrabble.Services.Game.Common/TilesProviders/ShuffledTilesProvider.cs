namespace SuperScrabble.Services.Game.Common.TilesProviders
{
    using SuperScrabble.Services.Common;

    public class ShuffledTilesProvider : BaseTilesProvider
    {
        private readonly IShuffleService shuffleService;
        private readonly ITilesProvider innerTilesProvider;

        public ShuffledTilesProvider(IShuffleService shuffleService, ITilesProvider innerTilesProvider)
        {
            this.shuffleService = shuffleService;
            this.innerTilesProvider = innerTilesProvider;
        }

        public override IEnumerable<Tile> GetAllWildcardOptions()
        {
            return this.innerTilesProvider.GetAllWildcardOptions();
        }

        public override IEnumerable<KeyValuePair<char, TileInfo>> GetTiles()
        {
            return this.shuffleService.Shuffle(this.innerTilesProvider.GetTiles());
        }
    }
}

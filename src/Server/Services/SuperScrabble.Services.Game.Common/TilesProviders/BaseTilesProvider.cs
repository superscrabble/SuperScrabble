namespace SuperScrabble.Services.Game.Common.TilesProviders
{
    using SuperScrabble.Common;

    public abstract class BaseTilesProvider : ITilesProvider
    {
        public virtual IEnumerable<Tile> GetAllWildcardOptions()
        {
            return this.GetTiles()
                       .Where(x => x.Key != GlobalConstants.WildcardValue)
                       .Select(x => new Tile(x.Key, x.Value.Points));
        }

        public abstract IEnumerable<KeyValuePair<char, TileInfo>> GetTiles();
    }
}

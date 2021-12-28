using SuperScrabble.Services.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.Services.Game.TilesProviders
{
    public abstract class BaseTilesProvider : ITilesProvider
    {
        protected IGameplayConstantsProvider gameplayConstantsProvider;

        public BaseTilesProvider(IGameplayConstantsProvider gameplayConstantsProvider)
        {
            this.gameplayConstantsProvider = gameplayConstantsProvider;
        }

        public virtual IEnumerable<Tile> GetAllWildcardOptions()
        {
            return this.GetTiles().Where(x => x.Key != this.gameplayConstantsProvider.WildcardValue)
                                  .Select(x => new Tile(x.Key, x.Value.Points));
        }

        public abstract IEnumerable<KeyValuePair<char, TileInfo>> GetTiles();
    }
}

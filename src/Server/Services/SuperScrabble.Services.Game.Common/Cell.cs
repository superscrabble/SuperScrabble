namespace SuperScrabble.Services.Game.Common
{
    public class Cell
    {
        public Cell(CellType type, Tile? tile = null)
        {
            this.Type = type;
            this.Tile = tile;
        }

        public CellType Type { get; }

        public Tile? Tile { get; set; }
    }
}

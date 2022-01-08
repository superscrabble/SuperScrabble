namespace SuperScrabble.WebApi.ViewModels.Game
{
    public class BoardViewModel
    {
        public IEnumerable<CellViewModel> Cells { get; set; } = default!;

        public int Width { get; set; }

        public int Height { get; set; }
    }
}

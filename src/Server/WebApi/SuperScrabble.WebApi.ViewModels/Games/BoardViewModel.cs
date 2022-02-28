namespace SuperScrabble.WebApi.ViewModels.Games
{
    public class BoardViewModel
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public IEnumerable<CellViewModel> Cells { get; set; } = default!;
    }
}

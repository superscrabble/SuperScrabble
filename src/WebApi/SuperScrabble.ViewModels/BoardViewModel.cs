namespace SuperScrabble.ViewModels
{
    using System.Collections.Generic;

    public class BoardViewModel
    {
        public IEnumerable<CellViewModel> Cells { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}

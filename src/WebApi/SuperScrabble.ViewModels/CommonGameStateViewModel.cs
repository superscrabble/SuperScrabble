namespace SuperScrabble.ViewModels
{
    using System.Collections.Generic;

    public class CommonGameStateViewModel
    {
        public int RemainingTilesCount { get; set; }

        public IEnumerable<KeyValuePair<string, int>> PointsByUserNames { get; set; }

        public BoardViewModel Board { get; set; }

        public string PlayerOnTurnUserName { get; set; }
    }
}

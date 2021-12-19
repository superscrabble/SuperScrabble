namespace SuperScrabble.ViewModels
{
    using System.Collections.Generic;

    public class GameSummaryViewModel
    {
        public IEnumerable<KeyValuePair<string, int>> PointsByUserNames { get; set; }

        public string GameOutcomeMessage { get; set; }

        public int GameOutcomeNumber { get; set; }
    }
}

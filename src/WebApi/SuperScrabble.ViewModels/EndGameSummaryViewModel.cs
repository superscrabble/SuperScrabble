namespace SuperScrabble.ViewModels
{
    using SuperScrabble.Models;
    using System.Collections.Generic;

    public class EndGameSummaryViewModel
    {
        public IEnumerable<KeyValuePair<string, int>> PointsByUserNames { get; set; }

        public GameOutcome GameOutcome { get; set; }
    }
}

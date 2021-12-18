namespace SuperScrabble.ViewModels
{
    using System.Collections.Generic;

    public class EndGameSummaryViewModel
    {
        public IEnumerable<KeyValuePair<string, int>> PointsByUserNames { get; set; }

        public string WinnerUserName { get; set; }
    }
}

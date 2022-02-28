namespace SuperScrabble.WebApi.ViewModels.Games
{
    public class GameSummaryViewModel
    {
        public IEnumerable<KeyValuePair<string, int>> PointsByUserNames { get; set; } = default!;

        public string GameOutcomeMessage { get; set; } = default!;

        public int GameOutcomeNumber { get; set; }
    }
}

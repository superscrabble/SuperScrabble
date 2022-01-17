namespace SuperScrabble.WebApi.ViewModels.Game
{
    public class CommonGameStateViewModel
    {
        public int RemainingTilesCount { get; set; }

        public IEnumerable<KeyValuePair<string, int>> PointsByUserNames { get; set; } = new List<KeyValuePair<string, int>>();

        public IEnumerable<string> UserNamesOfPlayersWhoHaveLeftTheGame { get; set; } = new List<string>();

        public BoardViewModel Board { get; set; } = default!;

        public string PlayerOnTurnUserName { get; set; } = default!;

        public bool IsTileExchangePossible { get; set; }

        public bool IsGameOver { get; set; }
    }
}

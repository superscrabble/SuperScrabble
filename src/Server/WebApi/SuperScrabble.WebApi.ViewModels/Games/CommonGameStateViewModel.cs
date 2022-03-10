using SuperScrabble.Services.Game.Common;

namespace SuperScrabble.WebApi.ViewModels.Games
{
    public class CommonGameStateViewModel
    {
        public Dictionary<string, int>? RemainingSecondsByUserNames { get; set; }

        public int RemainingTilesCount { get; set; }

        public IEnumerable<TeamViewModel> Teams { get; set; } = default!;

        public IEnumerable<string> UserNamesOfPlayersWhoHaveLeftTheGame { get; set; } = default!;

        public BoardViewModel Board { get; set; } = default!;

        public string PlayerOnTurnUserName { get; set; } = default!;

        public bool IsTileExchangePossible { get; set; }

        public bool IsGameOver { get; set; }

        public int MaxTimerSeconds { get; set; }

        public IEnumerable<GameHistoryLog> Logs { get; set; } = default!;
    }
}

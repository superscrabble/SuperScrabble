﻿namespace SuperScrabble.WebApi.ViewModels.Game
{
    public class CommonGameStateViewModel
    {
        public int RemainingTilesCount { get; set; }

        public IEnumerable<TeamViewModel> Teams { get; set; } = default!;

        public IEnumerable<string> UserNamesOfPlayersWhoHaveLeftTheGame { get; set; } = default!;

        public BoardViewModel Board { get; set; } = default!;

        public string PlayerOnTurnUserName { get; set; } = default!;

        public bool IsTileExchangePossible { get; set; }

        public bool IsGameOver { get; set; }
    }
}
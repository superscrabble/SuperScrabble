﻿namespace SuperScrabble.Services.Game.Models
{
    using SuperScrabble.Services.Game.Common.GameplayConstantsProviders;
    using SuperScrabble.Services.Game.Models.Boards;
    using SuperScrabble.Services.Game.Models.TilesBags;

    public class GameState
    {
        private readonly List<Player> players = new();
        private readonly IGameplayConstantsProvider gameplayConstantsProvider;

        public GameState(
            IEnumerable<KeyValuePair<string, string>> connectionIdsByUserNames,
            ITilesBag tilesBag,
            IBoard board,
            IGameplayConstantsProvider gameplayConstantsProvider)
        {
            foreach (var user in connectionIdsByUserNames)
            {
                this.players.Add(new Player(user.Key, 0, user.Value, gameplayConstantsProvider));
            }

            this.TilesBag = tilesBag;
            this.PlayerIndex = 0;
            this.Board = board;
            this.gameplayConstantsProvider = gameplayConstantsProvider;
            this.IsGameOver = false;
        }

        public Player CurrentPlayer => this.players[this.PlayerIndex];

        public bool IsGameOver { get; private set; }

        public ITilesBag TilesBag { get; }

        public IBoard Board { get; }

        public int PlayerIndex { get; private set; }

        public string? GroupName { get; set; }

        public IReadOnlyCollection<Player> Players => this.players.ToList().AsReadOnly();

        public int TilesCount => this.TilesBag.TilesCount;

        public bool IsTileExchangePossible =>
            this.TilesCount >= this.gameplayConstantsProvider.PlayerTilesCount;

        public void CheckForGameEnd()
        {
            int playersStillPlayingCount = 0;

            foreach (Player player in this.Players)
            {
                if (!player.HasLeftTheGame)
                {
                    playersStillPlayingCount++;
                }
            }

            if (playersStillPlayingCount <= 1)
            {
                this.EndGame();
            }
        }

        public Player? GetPlayer(string userName)
        {
            return this.players.FirstOrDefault(p => p.UserName == userName);
        }

        public void ResetAllPlayersConsecutiveSkipsCount()
        {
            foreach (Player player in this.Players)
            {
                player.ConsecutiveSkipsCount = 0;
            }
        }

        public void NextPlayer()
        {
            while (this.Players.Count > 1)
            {
                this.PlayerIndex++;

                if (this.PlayerIndex >= this.Players.Count)
                {
                    this.PlayerIndex = 0;
                }

                if (!this.CurrentPlayer.HasLeftTheGame)
                {
                    break;
                }
            }
        }

        public void EndGame()
        {
            this.IsGameOver = true;

            foreach (Player player in this.Players)
            {
                player.SubtractRemainingTilesPoints();
            }
        }
    }
}

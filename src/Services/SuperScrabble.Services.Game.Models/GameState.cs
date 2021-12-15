﻿namespace SuperScrabble.Services.Game.Models
{
    using System.Linq;
    using System.Collections.Generic;

    public class GameState
    {
        private readonly List<Player> players = new();

        public GameState(
            IEnumerable<KeyValuePair<string, string>> connectionIdsByUserNames,
            TilesBag tilesBag,
            IBoard board)
        {
            foreach (var user in connectionIdsByUserNames)
            {
                this.players.Add(new Player(user.Key, 0, user.Value));
            }

            this.TilesBag = tilesBag;
            this.PlayerIndex = 0;
            this.Board = board;
        }

        public Player CurrentPlayer => this.players[this.PlayerIndex];

        public TilesBag TilesBag { get; }

        public IBoard Board { get; }

        public int PlayerIndex { get; private set; }

        public IReadOnlyCollection<Player> Players => this.players.ToList().AsReadOnly();

        public int TilesCount => this.TilesBag.TilesCount;

        public Player GetPlayer(string userName)
        {
            return this.players.FirstOrDefault(p => p.UserName == userName);
        }

        public void NextPlayer()
        {
            this.PlayerIndex++;

            if (this.PlayerIndex >= this.Players.Count)
            {
                this.PlayerIndex = 0;
            }
        }
    }
}

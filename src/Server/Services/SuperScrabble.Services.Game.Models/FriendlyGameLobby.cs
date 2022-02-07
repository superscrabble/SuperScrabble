using SuperScrabble.Services.Game.Common;

namespace SuperScrabble.Services.Game.Models
{
    public class FriendlyGameLobby
    {
        public const int MaxPlayersCount = 4;
        public const int MinPlayersCount = 2;

        public FriendlyGameLobby(Player owner, FriendlyGameConfiguration gameConfig)
        {
            this.Owner = owner;
            this.GameConfig = gameConfig;
            this.LobbyMembers.Add(this.Owner);
        }

        public Player Owner { get; private set; }

        public List<Player> LobbyMembers { get; } = new();

        public FriendlyGameConfiguration GameConfig { get; }

        public bool IsFull => this.LobbyMembers.Count >= MaxPlayersCount;

        public bool IsAbleToStartGame => this.LobbyMembers.Count >= MinPlayersCount;
    }
}

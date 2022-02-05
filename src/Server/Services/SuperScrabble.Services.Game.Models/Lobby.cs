using SuperScrabble.Services.Game.Common;

namespace SuperScrabble.Services.Game.Models
{
    public class Lobby
    {
        public Lobby(Player creator)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Creator = creator;
        }

        public string Id { get; set; }

        public Player Creator { get; set; }

        public GameRoomConfiguration RoomConfiguration { get; set; }
    }
}

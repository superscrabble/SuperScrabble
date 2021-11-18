namespace SuperScrabble.WebApi.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GameHub : Hub
    {
        public static readonly HashSet<string> WaitingPlayers = new();

        public async Task JoinRoom()
        {
            string id = Context.ConnectionId;

            WaitingPlayers.Add(id);

            if (WaitingPlayers.Count >= 4)
            {
                // create game service instance
                // add players to the game
                // sendMessage("StartGame", gameState)
            }
            else
            {
                await Groups.AddToGroupAsync(id, "WaitingPlayers");
                int neededPlayersCount = 4 - WaitingPlayers.Count;
                await Clients.Groups("WaitingPlayers").SendAsync("WaitingForMorePlayers", neededPlayersCount);
            }

            // check for already waiting players/rooms
        }

        // click start game button -> loading screen -> start game
    }
}

namespace SuperScrabble.WebApi.Hubs
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GameHub : Hub
    {
        public static readonly Dictionary<string, string> WaitingPlayers = new();

        [Authorize]
        public async Task JoinRoom()
        {
            string id = Context.ConnectionId;

            string userName = Context.User.Identity.Name;

            WaitingPlayers[userName] = id;

            if (WaitingPlayers.Count >= 2)
            {
                string groupName = Guid.NewGuid().ToString();

                foreach (var waitingPlayer in WaitingPlayers)
                {
                    await this.Groups.AddToGroupAsync(waitingPlayer.Value, groupName);
                }

                await this.Clients.Group(groupName).SendAsync("StartGame", WaitingPlayers);
                WaitingPlayers.Clear();
                // create game service instance
                // add players to the game
                // sendMessage("StartGame", gameState)
            }
            else
            {
                //await Groups.AddToGroupAsync(id, "WaitingPlayers");
                //int neededPlayersCount = 4 - WaitingPlayers.Count;
                //await Clients.Groups("WaitingPlayers").SendAsync("WaitingForMorePlayers", neededPlayersCount);
            }

            // check for already waiting players/rooms
        }

        // click start game button -> loading screen -> start game
    }
}

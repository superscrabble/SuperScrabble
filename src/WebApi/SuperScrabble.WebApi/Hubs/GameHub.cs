namespace SuperScrabble.WebApi.Hubs
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GameHub : Hub
    {
        public const int GamePlayersCount = 3;
        public static readonly Dictionary<string, string> WaitingPlayers = new();

        [Authorize]
        public async Task JoinRoom()
        {
            string id = Context.ConnectionId;

            string userName = Context.User.Identity.Name;

            if(WaitingPlayers.ContainsKey(userName) && WaitingPlayers[userName] == id)
            {
                return;
            }

            WaitingPlayers[userName] = id;

            if (WaitingPlayers.Count >= GamePlayersCount)
            {
                string groupName = Guid.NewGuid().ToString();

                foreach (var waitingPlayer in WaitingPlayers)
                {
                    await this.Groups.AddToGroupAsync(waitingPlayer.Value, groupName);
                    await this.Groups.RemoveFromGroupAsync(waitingPlayer.Value, nameof(WaitingPlayers));
                }

                await this.Clients.Group(groupName).SendAsync("StartGame", WaitingPlayers);
                WaitingPlayers.Clear();
                // create game service instance
                // add players to the game
                // sendMessage("StartGame", gameState)

                //TODO: ensure that race condition will not happen when accessing waitingPlayers
            }
            else
            {
                await Groups.AddToGroupAsync(id, nameof(WaitingPlayers));
                int neededPlayersCount = GamePlayersCount - WaitingPlayers.Count;
                await this.Clients.Groups(nameof(WaitingPlayers)).SendAsync("WaitingForMorePlayers", neededPlayersCount);
            }

            // check for already waiting players/rooms
        }

        //LeaveQueue() - remove from WaitingPlayer
        //LeaveRoom() - remove from current Game
        //Game - writeWord(), 

        // click start game button -> loading screen -> start game
    }
}

namespace SuperScrabble.WebApi.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    using SuperScrabble.Services.Game;

    public class GameHub : Hub
    {
        public const int GamePlayersCount = 3;
        public static readonly Dictionary<string, string> WaitingPlayers = new();
        public static readonly Dictionary<string, GameState> GamesByGroupName = new();
        public static readonly Dictionary<string, string> GroupsByUserName = new();

        private readonly IGameService gameService;

        public GameHub(IGameService gameService)
        {
            this.gameService = gameService;
        }


        [Authorize]
        public async Task JoinRoom()
        {
            string id = Context.ConnectionId;
            string userName = Context.User.Identity.Name;

            if (WaitingPlayers.ContainsKey(userName) && WaitingPlayers[userName] == id)
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
                    GroupsByUserName.Add(waitingPlayer.Key, groupName);
                }

                var gameState = this.gameService.CreateGame(WaitingPlayers.Keys);
                GamesByGroupName.Add(groupName, gameState);

                await this.Clients.Group(groupName).SendAsync("StartGame", GamesByGroupName[groupName]);
                WaitingPlayers.Clear();

                //TODO: ensure that race condition will not happen when accessing waitingPlayers
            }
            else
            {
                await Groups.AddToGroupAsync(id, nameof(WaitingPlayers));
                await SendNeededPlayersCountAsync(nameof(WaitingPlayers));
            }

            // check for already waiting players/rooms
        }

        [Authorize]
        public async Task LeaveQueue()
        {
            string id = Context.ConnectionId;
            string userName = Context.User.Identity.Name;

            WaitingPlayers.Remove(userName);
            await this.Groups.RemoveFromGroupAsync(id, nameof(WaitingPlayers));
            await SendNeededPlayersCountAsync(nameof(WaitingPlayers));
        }

        private async Task SendNeededPlayersCountAsync(string groupName)
        {
            int neededPlayersCount = GamePlayersCount - WaitingPlayers.Count;
            await this.Clients.Groups(groupName).SendAsync("WaitingForMorePlayers", neededPlayersCount);
        }

        //LeaveRoom() - remove from current Game
        //Game - writeWord(), 

        // click start game button -> loading screen -> start game
    }
}

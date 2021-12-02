namespace SuperScrabble.WebApi.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    using SuperScrabble.Services.Game;
    using SuperScrabble.Services.Game.Models;

    public class GameHub : Hub
    {
        public const int GamePlayersCount = 2;
        public static readonly Dictionary<string, string> WaitingConnectionIdsByUserName = new();
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

            if (WaitingConnectionIdsByUserName.ContainsKey(userName) 
                && WaitingConnectionIdsByUserName[userName] == id)
            {
                return;
            }

            if (GroupsByUserName.ContainsKey(userName))
            {
                // TODO: Send appropriate message to the user
                return;
            }

            WaitingConnectionIdsByUserName[userName] = id;

            if (WaitingConnectionIdsByUserName.Count >= GamePlayersCount)
            {
                string groupName = Guid.NewGuid().ToString();

                foreach (var waitingPlayer in WaitingConnectionIdsByUserName)
                {
                    await this.Groups.AddToGroupAsync(waitingPlayer.Value, groupName);
                    await this.Groups.RemoveFromGroupAsync(waitingPlayer.Value, nameof(WaitingConnectionIdsByUserName));

                    GroupsByUserName.Add(waitingPlayer.Key, groupName);
                }

                var gameState = this.gameService.CreateGame(WaitingConnectionIdsByUserName);
                GamesByGroupName.Add(groupName, gameState);

                await this.Clients.Group(groupName).SendAsync("StartGame", groupName);
                await this.UpdateGameStateAsync(groupName);

                WaitingConnectionIdsByUserName.Clear();

                //TODO: ensure that race condition will not happen when accessing waitingPlayers
            }
            else
            {
                await Groups.AddToGroupAsync(id, nameof(WaitingConnectionIdsByUserName));
                await SendNeededPlayersCountAsync(nameof(WaitingConnectionIdsByUserName));
            }

            // check for already waiting players/rooms
        }

        [Authorize]
        public async Task LoadGame(string groupName)
        {
            string userName = Context.User.Identity.Name;

            if(GroupsByUserName.ContainsKey(userName) && GroupsByUserName[userName] == groupName)
            {
                GameState gameState = GamesByGroupName[groupName];
                var viewModel = this.gameService.MapFromGameState(gameState, userName);
                await this.Clients.Client(this.Context.ConnectionId).SendAsync("UpdateGameState", viewModel);
            }
        }
        
        private async Task UpdateGameStateAsync(string groupName)
        {
            GameState gameState = GamesByGroupName[groupName];

            foreach (Player player in gameState.Players)
            {
                this.gameService.FillPlayerTiles(gameState, player.UserName);
            }

            foreach (Player player in gameState.Players)
            {
                var viewModel = this.gameService.MapFromGameState(gameState, player.UserName);
                await this.Clients.Client(player.ConnectionId).SendAsync("UpdateGameState", viewModel);
            }
        }

        [Authorize]
        public async Task LeaveQueue()
        {
            string id = Context.ConnectionId;
            string userName = Context.User.Identity.Name;

            WaitingConnectionIdsByUserName.Remove(userName);
            await this.Groups.RemoveFromGroupAsync(id, nameof(WaitingConnectionIdsByUserName));
            await SendNeededPlayersCountAsync(nameof(WaitingConnectionIdsByUserName));
        }

        private async Task SendNeededPlayersCountAsync(string groupName)
        {
            int neededPlayersCount = GamePlayersCount - WaitingConnectionIdsByUserName.Count;
            await this.Clients.Groups(groupName).SendAsync("WaitingForMorePlayers", neededPlayersCount);
        }

        //LeaveRoom() - remove from current Game
        //Game - writeWord(), 

        // click start game button -> loading screen -> start game
    }
}

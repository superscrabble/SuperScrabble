namespace SuperScrabble.WebApi.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Services.Game;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.ViewModels;

    public class GameHub : Hub
    {
        public const string WaitingPlayersQueueGroupName = "WaitingPlayerQueue";
        public const string StartGameMethodName = "StartGame";

        private readonly IGameService gameService;
        private readonly IGameStateManager gameStateManager;

        public GameHub(IGameService gameService, IGameStateManager gameStateManager)
        {
            this.gameService = gameService;
            this.gameStateManager = gameStateManager;
        }

        public string UserName => this.Context.User.Identity.Name;

        [Authorize]
        public async Task WriteWord(WriteWordInputModel input)
        {
            GameState gameState = this.gameStateManager.GetGameState(this.UserName);
            WriteWordResult result = this.gameService.WriteWord(gameState, input, this.UserName);

            if (!result.IsSucceeded)
            {
                await this.Clients.Client(this.Context.ConnectionId).SendAsync("InvalidWriteWordInput", result);
            }
            else
            {
                string groupName = this.gameStateManager.GetGroupName(this.UserName);
                await this.UpdateGameStateAsync(groupName);
            }
        }

        [Authorize]
        public async Task JoinRoom()
        {
            string connectionId = Context.ConnectionId;
            string userName = Context.User.Identity.Name;

            if (this.gameStateManager.IsUserAlreadyWaiting(userName, connectionId)
                || this.gameStateManager.IsUserAlreadyInsideGame(userName))
            {
                return;
            }

            this.gameStateManager.AddUserToWaitingList(userName, connectionId);

            if (!this.gameStateManager.IsWaitingQueueFull)
            {
                await this.Groups.AddToGroupAsync(connectionId, WaitingPlayersQueueGroupName);
                await this.SendNeededPlayersCountAsync(WaitingPlayersQueueGroupName);
                return;
            }

            string groupName = this.gameStateManager.CreateGroupFromWaitingPlayers();
            var waitingPlayers = this.gameStateManager.GetWaitingPlayers(groupName);

            foreach (var waitingPlayer in waitingPlayers)
            {
                string waitingPlayerConnectionId = waitingPlayer.Value;

                await this.Groups.AddToGroupAsync(waitingPlayerConnectionId, groupName);
                await this.Groups.RemoveFromGroupAsync(waitingPlayerConnectionId, WaitingPlayersQueueGroupName);
            }

            GameState gameState = this.gameService.CreateGame(waitingPlayers);
            this.gameStateManager.AddGameStateToGroup(gameState, groupName);

            await this.Clients.Group(groupName).SendAsync(StartGameMethodName, groupName);
            await this.UpdateGameStateAsync(groupName);

            this.gameStateManager.ClearWaitingQueue();
        }

        [Authorize]
        public async Task LoadGame(string groupName)
        {
            string userName = Context.User.Identity.Name;

            if (this.gameStateManager.IsUserInsideGroup(userName, groupName))
            {
                GameState gameState = this.gameStateManager.GetGameState(userName);
                var viewModel = this.gameService.MapFromGameState(gameState, userName);
                await this.Clients.Client(this.Context.ConnectionId).SendAsync("UpdateGameState", viewModel);
            }
        }

        // TODO: Player order + timer
        // TODO: onDisconnected edge cases
        // TODO: handle unauthenticated users
        // TODO: remove player from waiting list and started games when logging out
        // TODO: onConnected -> check timed out players (GameStateManager)
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (this.UserName != null)
            {
                this.gameStateManager.RemoveUserFromWaitingQueue(this.UserName);
            }

            return Task.CompletedTask;
        }

        private async Task UpdateGameStateAsync(string groupName)
        {
            GameState gameState = this.gameStateManager.GetGameStateByGroupName(groupName);

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
            string connectionId = Context.ConnectionId;
            string userName = Context.User.Identity.Name;

            this.gameStateManager.RemoveUserFromWaitingQueue(userName);
            await this.Groups.RemoveFromGroupAsync(connectionId, WaitingPlayersQueueGroupName);
            await SendNeededPlayersCountAsync(WaitingPlayersQueueGroupName);
        }

        private async Task SendNeededPlayersCountAsync(string groupName)
        {
            await this.Clients.Groups(groupName)
                .SendAsync("WaitingForMorePlayers", this.gameStateManager.NeededPlayersCount);
        }
    }
}

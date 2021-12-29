namespace SuperScrabble.WebApi.Hubs
{
    using System;
    using System.Timers;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.DependencyInjection;

    using SuperScrabble.ViewModels;
    using SuperScrabble.Services.Data;
    using SuperScrabble.Services.Game;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.TilesProviders;
    using SuperScrabble.Services.Game.GameStateManagers;
    
    public class GameHub : Hub
    {
        private static readonly ConcurrentDictionary<Timer, GameState> gameStatesByTimers = new();

        public const string WaitingPlayersQueueGroupName = "WaitingPlayerQueue";
        public const string StartGameMethodName = "StartGame";
        public const string UpdateGameStateMethodName = "UpdateGameState";
        public const string UserAlreadyInsideGameMethodName = "UserAlreadyInsideGame";

        private readonly IGameService gameService;
        private readonly IGameStateManager gameStateManager;
        private readonly IGamesService gamesService;
        private readonly ITilesProvider tilesProvider;
        private readonly IServiceProvider serviceProvider;

        public GameHub(
            IGameService gameService,
            IGameStateManager gameStateManager,
            IGamesService gamesService,
            ITilesProvider tilesProvider,
            IServiceProvider serviceProvider)
        {
            this.gameService = gameService;
            this.gameStateManager = gameStateManager;
            this.gamesService = gamesService;
            this.tilesProvider = tilesProvider;
            this.serviceProvider = serviceProvider;
        }

        public string ConnectionId => this.Context.ConnectionId;

        public string UserName => this.Context.User.Identity.Name;

        public string GroupName => this.gameStateManager.GetGroupName(this.UserName);

        public GameState GameState => this.gameStateManager.GetGameState(this.UserName);

        [Authorize]
        public async Task LeaveGame()
        {
            if (this.GameState == null)
            {
                return;
            }

            Player rageQuitter = this.GameState.GetPlayer(this.UserName);
            rageQuitter.LeaveGame();

            this.GameState.CheckForGameEnd();
            this.GameState.NextPlayer();

            await this.SaveGameIfTheGameIsOverAsync();
            await this.UpdateGameStateAsync(this.GroupName);
        }

        [Authorize]
        public async Task WriteWord(WriteWordInputModel input)
        {
            GameTimer timer = GameTimer.GameTimersByGroupNames[this.GroupName];
            timer.Stop();

            GameOperationResult result = this.gameService.WriteWord(this.GameState, input, this.UserName);

            if (!result.IsSucceeded)
            {
                timer.Start();
                await this.SendValidationErrorMessageAsync("InvalidWriteWordInput", result);
                return;
            }

            await this.SaveGameIfTheGameIsOverAsync();
            await this.UpdateGameStateAsync(this.GroupName);

            timer.Reset();
            timer.Start();
        }

        [Authorize]
        public async Task ExchangeTiles(ExchangeTilesInputModel input)
        {
            GameOperationResult result = this.gameService.ExchangeTiles(this.GameState, input, this.UserName);

            if (!result.IsSucceeded)
            {
                await this.SendValidationErrorMessageAsync("InvalidExchangeTilesInput", result);
                return;
            }

            await this.UpdateGameStateAsync(this.GroupName);
        }

        [Authorize]
        public async Task SkipTurn()
        {
            GameOperationResult result = this.gameService.SkipTurn(this.GameState, this.UserName);
            
            if (!result.IsSucceeded)
            {
                await this.SendValidationErrorMessageAsync("ImpossibleToSkipTurn", result);
                return;
            }

            await this.SaveGameIfTheGameIsOverAsync();
            await this.UpdateGameStateAsync(this.GroupName);
        }

        [Authorize]
        public async Task GetAllWildcardOptions()
        {
            var options = this.tilesProvider.GetAllWildcardOptions();
            await this.Clients.Caller.SendAsync("ReceiveAllWildcardOptions", options);
        }

        [Authorize]
        public async Task JoinRoom()
        {
            if (this.gameStateManager.IsUserAlreadyWaiting(this.UserName, this.ConnectionId))
            {
                return;
            }

            if (this.gameStateManager.IsUserAlreadyInsideGame(this.UserName))
            {
                await this.Clients.Caller.SendAsync(UserAlreadyInsideGameMethodName, this.GroupName);
                return;
            }

            this.gameStateManager.AddUserToWaitingList(this.UserName, this.ConnectionId);

            if (!this.gameStateManager.IsWaitingQueueFull)
            {
                await this.Groups.AddToGroupAsync(this.ConnectionId, WaitingPlayersQueueGroupName);
                await this.SendNeededPlayersCountAsync(WaitingPlayersQueueGroupName);
                return;
            }

            string groupName = this.gameStateManager.CreateGroupFromWaitingPlayers();
            var waitingPlayers = this.gameStateManager.GetWaitingPlayers();

            foreach (var waitingPlayer in waitingPlayers)
            {
                string waitingPlayerConnectionId = waitingPlayer.Value;

                await this.Groups.AddToGroupAsync(waitingPlayerConnectionId, groupName);
                await this.Groups.RemoveFromGroupAsync(waitingPlayerConnectionId, WaitingPlayersQueueGroupName);
            }

            GameState gameState = this.gameService.CreateGame(waitingPlayers);
            this.gameStateManager.AddGameStateToGroup(gameState, groupName);

            GameTimer timer = ActivatorUtilities.CreateInstance<GameTimer>(this.serviceProvider, gameState);
            GameTimer.GameTimersByGroupNames.Add(groupName, timer);

            this.gameStateManager.ClearWaitingQueue();

            await this.Clients.Group(groupName).SendAsync(StartGameMethodName, groupName);
            await this.UpdateGameStateAsync(groupName);
            
            timer.Start();
        }

        [Authorize]
        public async Task LoadGame(string groupName)
        {
            if (this.gameStateManager.IsUserInsideGroup(this.UserName, groupName))
            {
                var viewModel = this.gameService.MapFromGameState(this.GameState, this.UserName);
                await this.Clients.Client(this.Context.ConnectionId).SendAsync(UpdateGameStateMethodName, viewModel);
            }
        }

        [Authorize]
        public async Task LeaveQueue()
        {
            this.gameStateManager.RemoveUserFromWaitingQueue(this.UserName);
            await this.Groups.RemoveFromGroupAsync(this.ConnectionId, WaitingPlayersQueueGroupName);
            await this.SendNeededPlayersCountAsync(WaitingPlayersQueueGroupName);
        }

        [Authorize]
        public override Task OnConnectedAsync()
        {
            if (!this.gameStateManager.IsUserAlreadyInsideGame(this.UserName))
            {
                return Task.CompletedTask;
            }

            GameState gameState = this.gameStateManager.GetGameState(this.UserName);

            if (gameState != null)
            {
                Player player = gameState.GetPlayer(this.UserName);

                if (player.ConnectionId != this.ConnectionId)
                {
                    player.ConnectionId = this.ConnectionId;
                }
            }

            this.Clients.Caller.SendAsync(UserAlreadyInsideGameMethodName, this.GroupName).GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (this.UserName != null)
            {
                this.gameStateManager.RemoveUserFromWaitingQueue(this.UserName);
            }

            return Task.CompletedTask;
        }

        private async Task SaveGameIfTheGameIsOverAsync()
        {
            if (this.GameState.IsGameOver)
            {
                var saveGameInput = new SaveGameInputModel
                {
                    Players = this.GameState.Players,
                    GameId = this.GroupName,
                };

                await this.gamesService.SaveGameAsync(saveGameInput);
            }
        }

        private async Task SendValidationErrorMessageAsync(string methodName, GameOperationResult result)
        {
            await this.Clients.Caller.SendAsync(methodName, result);
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
                await this.Clients.Client(player.ConnectionId).SendAsync(UpdateGameStateMethodName, viewModel);

                if (gameState.IsGameOver)
                {
                    this.gameStateManager.RemoveUserFromGroup(player.UserName);
                    await this.Groups.RemoveFromGroupAsync(player.ConnectionId, groupName);
                }
            }

            if (gameState.IsGameOver)
            {
                this.gameStateManager.RemoveGameState(groupName);
            }
        }

        private async Task SendNeededPlayersCountAsync(string groupName)
        {
            await this.Clients.Groups(groupName).SendAsync("WaitingForMorePlayers", this.gameStateManager.NeededPlayersCount);
        }
    }
}

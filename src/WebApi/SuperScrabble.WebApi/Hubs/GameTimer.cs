namespace SuperScrabble.WebApi.Hubs
{
    using System.Timers;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.SignalR;

    using SuperScrabble.ViewModels;

    using SuperScrabble.Services.Game;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.GameStateManagers;

    public class GameTimer : Timer
    {
        public static readonly Dictionary<string, GameTimer> GameTimersByGroupNames = new();

        private readonly GameState gameState;
        private readonly IGameService gameService;
        private readonly IGameStateManager gameStateManager;
        private readonly IHubContext<GameHub> hubContext;
        private readonly IGameplayConstantsProvider gameplayConstantsProvider;

        private int remainingGameTimerSeconds;

        public GameTimer(
            GameState gameState,
            IGameService gameService,
            IGameStateManager gameStateManager,
            IHubContext<GameHub> hubContext,
            IGameplayConstantsProvider gameplayConstantsProvider)
        {
            this.gameState = gameState;
            this.gameService = gameService;
            this.gameStateManager = gameStateManager;
            this.hubContext = hubContext;
            this.gameplayConstantsProvider = gameplayConstantsProvider;

            this.Reset();

            this.Elapsed += async (sender, args) => await OnTimedEvent(sender, args);
        }

        public void Reset()
        {
            this.AutoReset = true;
            this.Interval = 1000;
            this.remainingGameTimerSeconds = this.gameplayConstantsProvider.GameTimerSeconds;
        }

        private async Task OnTimedEvent(object sender, ElapsedEventArgs args)
        {
            if (remainingGameTimerSeconds >= 0)
            {
                int minutes = this.remainingGameTimerSeconds / 60;
                int seconds = this.remainingGameTimerSeconds % 60;

                var viewModel = new UpdateGameTimerViewModel
                {
                    Minutes = minutes,
                    Seconds = seconds,
                };

                await this.hubContext
                    .Clients.Group(this.gameState.GroupName)
                    .SendAsync("UpdateGameTimer", viewModel);

                this.remainingGameTimerSeconds--;
                return;
            }

            this.gameState.NextPlayer();

            foreach (Player player in this.gameState.Players)
            {
                this.gameService.FillPlayerTiles(gameState, player.UserName);
            }

            foreach (Player player in gameState.Players)
            {
                var viewModel = this.gameService.MapFromGameState(gameState, player.UserName);

                await this.hubContext.Clients
                    .Client(player.ConnectionId)
                    .SendAsync(GameHub.UpdateGameStateMethodName, viewModel);

                if (gameState.IsGameOver)
                {
                    this.gameStateManager.RemoveUserFromGroup(player.UserName);

                    await this.hubContext.Groups
                        .RemoveFromGroupAsync(
                            player.ConnectionId, this.gameState.GroupName);
                }
            }

            if (gameState.IsGameOver)
            {
                this.gameStateManager.RemoveGameState(this.gameState.GroupName);
            }

            this.Reset();
        }
    }
}

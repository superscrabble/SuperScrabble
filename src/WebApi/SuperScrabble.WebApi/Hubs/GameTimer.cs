namespace SuperScrabble.WebApi.Hubs
{
    using System.Timers;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.SignalR;

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
            this.Interval = this.gameplayConstantsProvider.GameTimerSeconds * 1000;
        }

        public async Task OnTimedEvent(object sender, ElapsedEventArgs args)
        {
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
                        .RemoveFromGroupAsync(player.ConnectionId, this.gameState.GroupName);
                }
            }

            if (gameState.IsGameOver)
            {
                this.gameStateManager.RemoveGameState(this.gameState.GroupName);
            }
        }
    }
}

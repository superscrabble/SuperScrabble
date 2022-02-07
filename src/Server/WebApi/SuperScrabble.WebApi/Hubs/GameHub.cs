namespace SuperScrabble.WebApi.Hubs
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    using SuperScrabble.Common.Exceptions.Matchmaking;
    using SuperScrabble.Services.Game;
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Matchmaking;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.WebApi.ViewModels.Game;

    [Authorize]
    public class GameHub : Hub
    {
        private static class Messages
        {
            public const string Error = "Error";
            public const string StartGame = "StartGame";
            public const string UpdateGameState = "UpdateGameState";
            public const string ReceiveFriendlyGameCode = "ReceiveFriendlyGameCode";
            public const string PlayerJoinedLobby = "PlayerJoinedLobby";
            public const string EnableFriendlyGameStart = "EnableFriendlyGameStart";
        }

        private readonly IMatchmakingService matchmakingService;
        private readonly IGameService gameService;

        public GameHub(IMatchmakingService matchmakingService, IGameService gameService)
        {
            this.matchmakingService = matchmakingService;
            this.gameService = gameService;
        }

        public string? UserName => this.Context.User?.Identity?.Name;

        public string ConnectionId => this.Context.ConnectionId;

        [Authorize]
        public async Task CreateFriendlyGame(FriendlyGameConfiguration gameConfig)
        {
            string invitationCode = this.matchmakingService
                .CreateFriendlyGame(this.UserName!, this.ConnectionId, gameConfig);

            await this.Clients.Caller.SendAsync(Messages.ReceiveFriendlyGameCode, invitationCode);
        }

        [Authorize]
        public async Task JoinFriendlyGame(string invitationCode)
        {
            try
            {
                this.matchmakingService.JoinFriendlyGame(this.UserName!, this.ConnectionId, invitationCode);
                var gameLobby = this.matchmakingService.GetFriendlyGameLobby(invitationCode);

                if (gameLobby.IsAbleToStartGame)
                {
                    await this.Clients
                        .Client(gameLobby.Owner.ConnectionId)
                        .SendAsync(Messages.EnableFriendlyGameStart);
                }

                var connectionIds = gameLobby.LobbyMembers
                        .Where(mem => mem.UserName != this.UserName).Select(mem => mem.ConnectionId);

                await this.Clients.Clients(connectionIds).SendAsync(Messages.PlayerJoinedLobby, this.UserName);
            }
            catch (MatchmakingFailedException ex)
            {
                await this.Clients.Caller.SendAsync(Messages.Error, ex.ErrorCode);
            }
        }

        [Authorize]
        public async Task StartFriendlyGame(string invitationCode)
        {
            try
            {
                this.matchmakingService.StartFriendlyGame(this.UserName!, invitationCode);
                var gameState = this.matchmakingService.GetGameState(this.UserName!);

                foreach (Player player in gameState.Teams.SelectMany(team => team.Players))
                {
                    this.gameService.FillPlayerTiles(gameState, player);
                    await this.Groups.AddToGroupAsync(player.ConnectionId, gameState.GroupName);
                }

                await this.Clients.Group(gameState.GroupName)
                    .SendAsync(Messages.StartGame, gameState.GroupName);

                await this.UpdateGameStateAsync(gameState);
            }
            catch (MatchmakingFailedException ex)
            {
                await this.Clients.Caller.SendAsync(Messages.Error, ex.ErrorCode);
            }
        }

        private async Task UpdateGameStateAsync(GameState gameState)
        {
            foreach (Team team in gameState.Teams)
            {
                foreach (Player player in team.Players)
                {
                    PlayerGameStateViewModel viewModel = this.gameService
                        .MapFromGameState(gameState, player.UserName);

                    await this.Clients.Client(player.ConnectionId)
                        .SendAsync(Messages.UpdateGameState, viewModel);
                }
            }
        }
    }
}

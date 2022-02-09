namespace SuperScrabble.WebApi.Hubs
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using SuperScrabble.Common.Exceptions.Matchmaking;
    using SuperScrabble.Services.Game;
    using SuperScrabble.Services.Game.Common;
    using SuperScrabble.Services.Game.Common.Enums;
    using SuperScrabble.Services.Game.Matchmaking;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.WebApi.ViewModels.Game;

    [Authorize]
    public class GameHub : Hub
    {
        private static class Messages
        {
            public const string StartGame = "StartGame";
            public const string UpdateGameState = "UpdateGameState";
        }

        /// <summary>
        /// Dictionary which stores information about all currently connected
        /// players where the key is the username and the value is the connection id
        /// </summary>
        private static readonly Dictionary<string, string> connectionIdsByUserName = new();

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
        public async Task JoinFriendlyGame(string invitationCode)
        {
            try
            {
                this.matchmakingService.JoinFriendlyGame(
                    this.UserName!, this.ConnectionId, invitationCode, out bool canGameBeStarted);

                if (!canGameBeStarted)
                {
                    return;
                }

                Console.WriteLine("YES");

                GameState? gameState = this.matchmakingService.GetGameState(this.UserName!)
                    ?? throw new ArgumentException(
                        $"No game state for the given {nameof(this.UserName)} was found.");

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
                await this.Clients.Caller.SendAsync("Error", ex.ErrorCode);
            }
        }

        [Authorize]
        public async Task CreateFriendlyGame(CreateFriendlyGameInputModel input)
        {
            string code = this.matchmakingService.CreateFriendlyGame(this.UserName!, this.ConnectionId, input);
            await this.Clients.Caller.SendAsync("ReceiveFriendlyGameCode", code);
        }

        private async Task JoinRoomAsync(GameRoomConfiguration input)
        {
            if (this.matchmakingService.IsUserAlreadyWaitingToJoinGame(this.UserName!))
            {
                //TODO: Send error message
                return;
            }

            if (this.matchmakingService.IsUserAlreadyInsideGame(this.UserName!))
            {
                //TODO: Send error message
                return;
            }

            int maxPlayersCount = (int)input.TeamType;

            if (input.TeamType == TeamType.Solo)
            {
                var teamToAdd = new Team(maxPlayersCount);
                teamToAdd.AddPlayer(this.UserName!, this.ConnectionId);

                this.matchmakingService.AddTeamToWaitingQueue(
                    input, teamToAdd, out bool hasGameStarted);

                if (!hasGameStarted)
                {
                    return;
                }

                GameState? gameState = this.matchmakingService.GetGameState(this.UserName!)
                    ?? throw new ArgumentException(
                        $"No game state for the given {nameof(this.UserName)} was found.");

                foreach (Player player in gameState.Teams.SelectMany(team => team.Players))
                {
                    this.gameService.FillPlayerTiles(gameState, player);
                    await this.Groups.AddToGroupAsync(player.ConnectionId, gameState.GroupName);
                }

                await this.Clients.Group(gameState.GroupName)
                    .SendAsync(Messages.StartGame, gameState.GroupName);

                await this.UpdateGameStateAsync(gameState);
            }
            else if (input.TeamType == TeamType.Duo)
            {
                // addToLobby
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

        public override async Task OnConnectedAsync()
        {
            if (this.UserName == null)
            {
                await this.SendUnauthorizedErrorAsync(this.ConnectionId);
                return;
            }

            if (!connectionIdsByUserName.ContainsKey(this.UserName!))
            {
                connectionIdsByUserName.Add(this.UserName!, this.ConnectionId);
            }
            else
            {
                connectionIdsByUserName[this.UserName!] = this.ConnectionId;
            }

            // TODO: Send all pending game invitations to the user
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (this.UserName == null)
            {
                await this.SendUnauthorizedErrorAsync(this.ConnectionId);
                return;
            }

            if (connectionIdsByUserName.ContainsKey(this.UserName!))
            {
                connectionIdsByUserName.Remove(this.UserName!);
            }
        }

        private async Task SendUnauthorizedErrorAsync(string connectionId)
        {
            await this.Clients.Client(connectionId).SendAsync("UnauthorizedHubClient");
        }
    }
}

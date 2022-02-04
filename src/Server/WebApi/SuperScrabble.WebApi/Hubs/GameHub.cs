namespace SuperScrabble.WebApi.Hubs
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using SuperScrabble.Services.Game.Common;

    [Authorize]
    public class GameHub : Hub
    {
        /// <summary>
        /// Dictionary which stores information about all currently connected
        /// players where the key is the username and the value is the connection id
        /// </summary>
        private static readonly Dictionary<string, string> connectionIdsByUserName = new();

        public string? UserName => this.Context.User?.Identity?.Name;

        public string ConnectionId => this.Context.ConnectionId;

        public async Task JoinRoom(GameRoomConfiguration input)
        {
            if (this.UserName == null)
            {
                await this.SendUnauthorizedErrorAsync(this.ConnectionId);
            }

            // is already inside a game
            // is already waiting
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

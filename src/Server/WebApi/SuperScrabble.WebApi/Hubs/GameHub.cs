namespace SuperScrabble.WebApi.Hubs
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    using SuperScrabble.Common.Exceptions.Matchmaking;

    using SuperScrabble.Services.Game;
    using SuperScrabble.Services.Game.Common.Enums;
    using SuperScrabble.Services.Game.Matchmaking;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.Models.Parties;

    using SuperScrabble.WebApi.ViewModels.Game;
    using SuperScrabble.WebApi.ViewModels.Party;

    [Authorize]
    public class GameHub : Hub
    {
        private static class Messages
        {
            //Party
            public const string PartyJoined = "PartyJoined";
            public const string PartyLeft = "PartyLeft";
            public const string PartyCreated = "PartyCreated";
            public const string NewPlayerJoinedParty = "NewPlayerJoinedParty";
            public const string PlayerHasLeftParty = "PlayerHasLeftParty";
            public const string ReceivePartyData = "ReceivePartyData";
            public const string EnablePartyStart = "EnablePartyStart";

            //Game
            public const string StartGame = "StartGame";
            public const string UpdateGameState = "UpdateGameState";

            //Common
            public const string Error = "Error";
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
        public async Task CreateParty(PartyType partyType)
        {
            try
            {
                string partyId = this.matchmakingService
                    .CreateParty(this.UserName!, this.ConnectionId, partyType);

                await this.Clients.Caller.SendAsync(Messages.PartyCreated, partyId);
            }
            catch (MatchmakingFailedException ex)
            {
                await this.Clients.Caller.SendAsync(Messages.Error, ex.ErrorCode);
            }
        }

        [Authorize]
        public async Task LoadParty(string partyId)
        {
            Party party = default!;

            try
            {
                party = this.matchmakingService.GetPartyById(partyId);
            }
            catch (MatchmakingFailedException ex)
            {
                await this.Clients.Caller.SendAsync(Messages.Error, ex.ErrorCode);
                return;
            }

            var viewModel = new FriendPartyViewModel
            {
                Owner = party.Owner!.UserName,
                InvitationCode = party.InvitationCode,
                IsOwner = party.Owner?.UserName == this.UserName,
                Members = party.Members.Select(mem => mem.UserName),
                PartyType = party is FriendParty ? PartyType.Friendly : PartyType.Duo,
                ConfigSettings = new ConfigSetting[]
                {
                    new ConfigSetting()
                    {
                        Name = nameof(TimerType),
                        Options =
                            Enum.GetValues(typeof(TimerType)).Cast<TimerType>()
                                .Select(value => new SettingOption
                            {
                                Name = value.ToString(),
                                Value = (int)value,
                            })
                    },
                    new ConfigSetting()
                    {
                        Name = nameof(TimerDifficulty),
                        Options =
                            Enum.GetValues(typeof(TimerDifficulty)).Cast<TimerDifficulty>()
                                .Select(value => new SettingOption
                            {
                                Name = value.ToString(),
                                Value = (int)value,
                            })
                    }
                }
            };

            await this.Clients.Caller.SendAsync(Messages.ReceivePartyData, viewModel);
        }

        [Authorize]
        public async Task JoinParty(string invitationCode)
        {
            try
            {
                this.matchmakingService.JoinParty(
                    this.UserName!, this.ConnectionId, invitationCode, out bool hasEnoughPlayersToStartGame);

                Party party = this.matchmakingService.GetPartyByInvitationCode(invitationCode);

                if (hasEnoughPlayersToStartGame)
                {
                    await this.Clients
                        .Client(party.Owner!.ConnectionId)
                        .SendAsync(Messages.EnablePartyStart);
                }

                var connectionIds = party.Members
                        .Where(mem => mem.UserName != this.UserName)
                        .Select(mem => mem.ConnectionId);

                await this.Clients.Caller.SendAsync(Messages.PartyJoined, party.Id);
                await this.Clients.Clients(connectionIds).SendAsync(Messages.NewPlayerJoinedParty, this.UserName);
            }
            catch (MatchmakingFailedException ex)
            {
                await this.Clients.Caller.SendAsync(Messages.Error, ex.ErrorCode);
            }
        }
        
        [Authorize]
        public async Task LeaveParty(string partyId)
        {
            try
            {
                this.matchmakingService.LeaveParty(
                    this.UserName!, partyId, out bool shouldDisposeParty);

                await this.Clients.Caller.SendAsync(Messages.PartyLeft);

                if (!shouldDisposeParty)
                {
                    Party party = this.matchmakingService.GetPartyById(partyId);

                    foreach (Member member in party.Members)
                    {
                        var viewModel = new PlayerHasLeftPartyViewModel
                        {
                            Owner = party.Owner?.UserName!,
                            IsOwner = party.Owner?.UserName == member.UserName,
                            RemainingMembers = party.Members.Select(mem => mem.UserName),
                            LeaverUserName = this.UserName!,
                        };

                        await this.Clients.Client(member.ConnectionId)
                            .SendAsync(Messages.PlayerHasLeftParty, viewModel);
                    }
                }

                if (shouldDisposeParty)
                {
                    this.matchmakingService.DisposeParty(partyId);
                }
            }
            catch (MatchmakingFailedException ex)
            {
                await this.Clients.Caller.SendAsync(Messages.Error, ex.ErrorCode);
            }
        }

        [Authorize]
        public async Task StartGameFromParty(string partyId)
        {
            try
            {
                this.matchmakingService.StartGameFromParty(this.UserName!, partyId, out bool hasGameStarted);

                if (!hasGameStarted)
                {
                    // Duo game -> redirect to loading screen
                    return;
                }

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

        [Authorize]
        public async Task SetFriendPartyConfiguration(FriendPartyConfig config, string partyId)
        {
            try
            {
                Party party = this.matchmakingService.GetPartyById(partyId);

                if (party.Owner?.UserName != this.UserName)
                {
                    throw new OnlyOwnerHasAccessException();
                }

                if (party is FriendParty friendParty)
                {
                    friendParty.TimerType = config.TimerType;
                    friendParty.TimerDifficulty = config.TimerDifficulty;
                }
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

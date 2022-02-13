﻿namespace SuperScrabble.WebApi.Hubs
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    using SuperScrabble.Common.Exceptions.Matchmaking;

    using SuperScrabble.Services.Game;
    using SuperScrabble.Services.Game.Common.Enums;
    using SuperScrabble.Services.Game.Common.TilesProviders;
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
            public const string UpdateFriendPartyConfigSettings = "UpdateFriendPartyConfigSettings";
            public const string PartyMemberConnectionIdChanged = "PartyMemberConnectionIdChanged";

            //Game
            public const string StartGame = "StartGame";
            public const string UpdateGameState = "UpdateGameState";
            public const string UserAlreadyInsideGame = "UserAlreadyInsideGame";

            //Common
            public const string Error = "Error";
        }

        private readonly IMatchmakingService matchmakingService;
        private readonly IGameService gameService;
        private readonly ITilesProvider tilesProvider;

        public GameHub(
            IMatchmakingService matchmakingService,
            IGameService gameService,
            ITilesProvider tilesProvider)
        {
            this.matchmakingService = matchmakingService;
            this.gameService = gameService;
            this.tilesProvider = tilesProvider;
        }

        public string? UserName => this.Context.User?.Identity?.Name;

        public string ConnectionId => this.Context.ConnectionId;

        public override async Task OnConnectedAsync()
        {
            if (this.matchmakingService.IsPlayerInsideGame(this.UserName!))
            {
                GameState gameState = this.matchmakingService.GetGameState(this.UserName!);
                Player player = gameState.GetPlayer(this.UserName!)!;

                if (player.ConnectionId == null)
                {
                    player.ConnectionId = this.ConnectionId;
                }

                await this.Clients.Caller.SendAsync(Messages.UserAlreadyInsideGame, gameState.GroupName);
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (this.matchmakingService.IsPlayerInsideGame(this.UserName!))
            {
                GameState gameState = this.matchmakingService.GetGameState(this.UserName!);
                Player player = gameState.GetPlayer(this.UserName!)!;

                if (player.ConnectionId == this.ConnectionId)
                {
                    player.ConnectionId = null;
                }
            }

            return Task.CompletedTask;
        }

        [Authorize]
        public async Task WriteWord(WriteWordInputModel input)
        {
            var gameState = this.matchmakingService.GetGameState(this.UserName!);
            GameOperationResult result = this.gameService.WriteWord(gameState, input, this.UserName!);

            if (!result.IsSucceeded)
            {
                await this.SendValidationErrorMessageAsync("InvalidWriteWordInput", result);
                return;
            }

            //await this.SaveGameIfTheGameIsOverAsync();
            await this.UpdateGameStateAsync(gameState);
        }

        [Authorize]
        public async Task LeaveGame()
        {
            var gameState = this.matchmakingService.GetGameState(this.UserName!);

            if (gameState == null)
            {
                return;
            }

            Player rageQuitter = gameState.GetPlayer(this.UserName!)!;
            rageQuitter.LeaveGame();

            gameState.EndGameIfRoomIsEmpty();

            gameState.CurrentTeam.NextPlayer();

            if (gameState.CurrentTeam.IsTurnFinished)
            {
                gameState.NextTeam();
            }

            //await this.SaveGameIfTheGameIsOverAsync();
            await this.UpdateGameStateAsync(gameState);
        }

        [Authorize]
        public async Task ExchangeTiles(ExchangeTilesInputModel input)
        {
            var gameState = this.matchmakingService.GetGameState(this.UserName!);
            GameOperationResult result = this.gameService.ExchangeTiles(gameState, input, this.UserName!);

            if (!result.IsSucceeded)
            {
                await this.SendValidationErrorMessageAsync("InvalidExchangeTilesInput", result);
                return;
            }

            await this.UpdateGameStateAsync(gameState);
        }

        [Authorize]
        public async Task SkipTurn()
        {
            var gameState = this.matchmakingService.GetGameState(this.UserName!);
            GameOperationResult result = this.gameService.SkipTurn(gameState, this.UserName!);

            if (!result.IsSucceeded)
            {
                await this.SendValidationErrorMessageAsync("ImpossibleToSkipTurn", result);
                return;
            }

            //await this.SaveGameIfTheGameIsOverAsync();
            await this.UpdateGameStateAsync(gameState);
        }

        [Authorize]
        public async Task GetAllWildcardOptions()
        {
            var options = this.tilesProvider.GetAllWildcardOptions();
            await this.Clients.Caller.SendAsync("ReceiveAllWildcardOptions", options);
        }

        [Authorize]
        public async Task JoinRoom(GameMode gameMode)
        {
            try
            {
                this.matchmakingService.JoinRoom(
                    this.UserName!, this.ConnectionId, gameMode, out bool hasGameStarted);

                if (hasGameStarted)
                {
                    await this.StartGameAsync();
                }
            }
            catch (MatchmakingFailedException ex)
            {
                await this.SendErrorAsync(ex.ErrorCode);
            }
        }

        [Authorize]
        public async Task JoinRandomDuo()
        {
            this.matchmakingService.JoinRandomDuoParty(
                this.UserName!, this.ConnectionId, out bool hasGameStarted);

            if (hasGameStarted)
            {
                await this.StartGameAsync();
            }
        }

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
                await this.SendErrorAsync(ex.ErrorCode);
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
                await this.SendErrorAsync(ex.ErrorCode);
                return;
            }

            const TimerType defaultTimerType = TimerType.Standard;

            var viewModel = new FriendPartyViewModel
            {
                Owner = party.Owner!.UserName,
                InvitationCode = party.InvitationCode,
                IsOwner = party.Owner?.UserName == this.UserName,
                Members = party.Members.Select(mem => mem.UserName),
                PartyType = party is FriendParty ? PartyType.Friendly : PartyType.Duo,
                ConfigSettings = new ConfigSetting[]
                {
                    CreateTimerTypeConfigSetting(defaultTimerType),

                    CreateTimerDifficultyConfigSetting(
                        TimerType.Standard, TimerDifficulty.Normal),
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
                    this.UserName!, this.ConnectionId,
                    invitationCode, out bool hasEnoughPlayersToStartGame);

                Party party = this.matchmakingService.GetPartyByInvitationCode(invitationCode);

                if (hasEnoughPlayersToStartGame)
                {
                    await this.Clients
                        .Client(party.Owner!.ConnectionId)
                        .SendAsync(Messages.EnablePartyStart);
                }

                await this.Clients.Caller.SendAsync(Messages.PartyJoined, party.Id);

                await this.Clients
                    .Clients(party.GetConnectionIds(this.UserName!))
                    .SendAsync(Messages.NewPlayerJoinedParty, this.UserName);
            }
            catch (PlayerAlreadyInsidePartyException)
            {
                Party party = this.matchmakingService.GetPartyByInvitationCode(invitationCode);
                Member? member = party.GetMember(this.UserName!);

                if (member != null)
                {
                    await this.Clients.Client(member.ConnectionId).SendAsync(Messages.PartyMemberConnectionIdChanged);
                    member.ConnectionId = this.ConnectionId;
                }

                await this.Clients.Caller.SendAsync(Messages.PartyJoined, party.Id);
            }
            catch (MatchmakingFailedException ex)
            {
                await this.SendErrorAsync(ex.ErrorCode);
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
                            IsPartyReady = party.HasEnoughPlayersToStartGame,
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
                await this.SendErrorAsync(ex.ErrorCode);
            }
        }

        [Authorize]
        public async Task StartGameFromParty(string partyId)
        {
            try
            {
                Party party = this.matchmakingService.GetPartyById(partyId);

                this.matchmakingService.StartGameFromParty(
                    this.UserName!, partyId, out bool hasGameStarted);

                if (!hasGameStarted)
                {
                    // Duo Game with friend
                    // TODO: Redirect to loading screen
                    return;
                }

                await this.StartGameAsync();
            }
            catch (MatchmakingFailedException ex)
            {
                await this.SendErrorAsync(ex.ErrorCode);
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

                    var configSettings = new ConfigSetting[]
                    {
                        CreateTimerTypeConfigSetting(friendParty.TimerType),

                        CreateTimerDifficultyConfigSetting(
                            friendParty.TimerType, friendParty.TimerDifficulty),
                    };

                    await this.Clients.Clients(friendParty.GetConnectionIds())
                        .SendAsync(Messages.UpdateFriendPartyConfigSettings, configSettings);
                }
            }
            catch (MatchmakingFailedException ex)
            {
                await this.SendErrorAsync(ex.ErrorCode);
            }
        }

        [Authorize]
        public async Task LoadGame(string groupName)
        {
            var gameState = this.matchmakingService.GetGameState(this.UserName!);
            var viewModel = this.gameService.MapFromGameState(gameState, this.UserName!);
            await this.Clients.Client(this.Context.ConnectionId).SendAsync(Messages.UpdateGameState, viewModel);
        }

        private async Task SendValidationErrorMessageAsync(string methodName, GameOperationResult result)
        {
            await this.Clients.Caller.SendAsync(methodName, result);
        }

        private async Task SendErrorAsync(string message)
        {
            await this.Clients.Caller.SendAsync(Messages.Error, message);
        }

        private async Task StartGameAsync()
        {
            var gameState = this.matchmakingService.GetGameState(this.UserName!);

            foreach (Player player in gameState.Teams.SelectMany(team => team.Players))
            {
                await this.Groups.AddToGroupAsync(player.ConnectionId, gameState.GroupName);
            }

            await this.Clients.Group(gameState.GroupName)
                .SendAsync(Messages.StartGame, gameState.GroupName);

            await this.UpdateGameStateAsync(gameState);
        }

        private async Task UpdateGameStateAsync(GameState gameState)
        {
            foreach (Team team in gameState.Teams)
            {
                foreach (Player player in team.Players)
                {
                    this.gameService.FillPlayerTiles(gameState, player);

                    PlayerGameStateViewModel viewModel = this.gameService
                        .MapFromGameState(gameState, player.UserName);

                    await this.Clients.Client(player.ConnectionId)
                        .SendAsync(Messages.UpdateGameState, viewModel);
                }
            }
        }

        private static ConfigSetting CreateTimerTypeConfigSetting(TimerType selectedTimerType)
        {
            return new ConfigSetting
            {
                Name = nameof(TimerType),
                Options = Enum
                    .GetValues(typeof(TimerType)).Cast<TimerType>()
                    .Select(value => new SettingOption
                    {
                        Value = (int)value,
                        Name = value.ToString(),
                        IsSelected = value == selectedTimerType,
                    })
            };
        }

        private static ConfigSetting CreateTimerDifficultyConfigSetting(
            TimerType selectedTimerType, TimerDifficulty selectedTimerDifficulty)
        {
            return new ConfigSetting
            {
                Name = nameof(TimerDifficulty),
                Options = Enum
                    .GetValues(typeof(TimerDifficulty)).Cast<TimerDifficulty>()
                    .Select(value => new SettingOption
                    {
                        Value = (int)value,
                        IsSelected = value == selectedTimerDifficulty,
                        Name = TimeSpan.FromSeconds(
                            value.GetSeconds(selectedTimerType)).ToString("mm':'ss"),
                    })
            };
        }
    }
}

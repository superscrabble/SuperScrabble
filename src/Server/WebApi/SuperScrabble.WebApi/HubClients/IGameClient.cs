namespace SuperScrabble.WebApi.HubClients;

using SuperScrabble.Services.Game.Common;

using SuperScrabble.WebApi.ViewModels.Game;
using SuperScrabble.WebApi.ViewModels.Party;

public interface IGameClient
{
    Task PartyJoined(string partyId);

    Task PartyCreated(string partyId);

    Task PartyLeft();

    Task NewPlayerJoinedParty(string joinedPlayerUserName);

    Task EnablePartyStart();

    Task ReceivePartyData(FriendPartyViewModel viewModel);

    Task UserAlreadyInsideGame(string gameId);

    Task StartGame(string gameId);

    Task UpdateGameState(PlayerGameStateViewModel viewModel);

    Task UserEnteredGameFromAnotherConnectionId();

    Task Error(string message);

    Task UpdateFriendPartyConfigSettings(IEnumerable<ConfigSetting> configSettings);

    Task PlayerHasLeftParty(PlayerHasLeftPartyViewModel viewModel);

    Task PartyMemberConnectionIdChanged();

    Task ReceiveAllWildcardOptions(IEnumerable<Tile> tiles);

    Task ImpossibleToSkipTurn(GameOperationResult result);

    Task InvalidExchangeTilesInput(GameOperationResult result);

    Task InvalidWriteWordInput(GameOperationResult result);
}

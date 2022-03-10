﻿namespace SuperScrabble.Services.Game.Matchmaking
{
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.Services.Game.Common.Enums;
    using SuperScrabble.Services.Game.Models.Parties;

    public interface IMatchmakingService
    {
        string CreateParty(string creatorUserName, string creatorConnectionId, PartyType partyType);

        void JoinParty(
            string joinerUserName, string joinerConnectionId,
            string invitationCode, out bool hasEnoughPlayersToStartGame);

        void StartGameFromParty(string starterUserName, string partyId, out bool hasGameStarted);

        void LeaveParty(string leaverUserName, string partyId, out bool shouldDisposeParty);

        void DisposeParty(string partyId);

        GameState GetGameState(string userName);

        Party GetPartyById(string partyId);

        Party GetPartyByInvitationCode(string invitationCode);

        void JoinRoom(string joinerUserName, string joinerConnectionId,
            GameMode gameMode, out bool hasGameStarted);

        void JoinRandomDuoParty(
            string joinerUserName, string joinerConnectionId, out bool hasGameStarted);

        bool IsUserInsideAnyGame(string userName);

        bool IsUserInsideGame(string userName, string gameId);

        void RemoveUserFromGame(string userName);

        void RemoveGameState(string gameId);

        bool IsUserInsideAnyParty(string userName);

        Party? GetPartyByUserName(string userName);

        IEnumerable<string> LeaveRoom(string userName);
    }
}

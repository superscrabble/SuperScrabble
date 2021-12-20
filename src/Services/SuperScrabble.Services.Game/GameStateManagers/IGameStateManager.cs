﻿namespace SuperScrabble.Services.Game.GameStateManagers
{
    using System.Collections.Generic;

    using SuperScrabble.Services.Game.Models;

    public interface IGameStateManager
    {
        GameState GetGameState(string userName);

        GameState GetGameStateByGroupName(string groupName);

        string GetGroupName(string userName);

        void RemoveUserFromGroup(string userName);

        bool IsUserAlreadyWaiting(string userName, string connectionId);

        bool IsUserAlreadyInsideGame(string userName);

        void AddUserToWaitingList(string userName, string connectionId);

        bool IsUserInsideGroup(string userName, string groupName);

        string CreateGroupFromWaitingPlayers();

        int WaitingPlayersCount { get; }

        bool IsWaitingQueueFull { get; }

        void ClearWaitingQueue();

        int NeededPlayersCount { get; }

        void AddGameStateToGroup(GameState gameState, string groupName);

        void RemoveUserFromWaitingQueue(string userName);

        void RemoveGameState(string groupName);

        IEnumerable<KeyValuePair<string, string>> GetWaitingPlayers(string groupName);
    }
}

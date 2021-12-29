namespace SuperScrabble.Services.Game.GameStateManagers
{
    using System;
    using System.Collections.Generic;

    using SuperScrabble.Services.Game.Models;

    public class StaticGameStateManager : IGameStateManager
    {
        private static readonly Dictionary<string, string> waitingConnectionIdsByUserName = new();
        private static readonly Dictionary<string, string> groupsByUserName = new();
        private static readonly Dictionary<string, GameState> gamesByGroupName = new();

        private readonly int playersInsideGameCount;
        private readonly IGameplayConstantsProvider gameplayConstantsProvider;

        public StaticGameStateManager(IGameplayConstantsProvider gameplayConstantsProvider)
        {
            this.gameplayConstantsProvider = gameplayConstantsProvider;
            this.playersInsideGameCount = this.gameplayConstantsProvider.PlayersPerGameCount;
        }

        public int WaitingPlayersCount =>
            waitingConnectionIdsByUserName.Count;

        public bool IsWaitingQueueFull =>
            this.WaitingPlayersCount >= this.playersInsideGameCount;

        public int NeededPlayersCount =>
            this.playersInsideGameCount - this.WaitingPlayersCount;

        public void AddGameStateToGroup(GameState gameState, string groupName)
        {
            if (gameState != null && groupName != null)
            {
                gameState.GroupName = groupName;
                gamesByGroupName[groupName] = gameState;
            }
        }

        public void AddUserToWaitingList(string userName, string connectionId)
        {
            waitingConnectionIdsByUserName.Add(userName, connectionId);
        }

        public void ClearWaitingQueue()
        {
            waitingConnectionIdsByUserName.Clear();
        }

        public string CreateGroupFromWaitingPlayers()
        {
            string groupName = Guid.NewGuid().ToString();

            foreach (var waitingPlayer in waitingConnectionIdsByUserName)
            {
                groupsByUserName.Add(waitingPlayer.Key, groupName);
            }

            return groupName;
        }

        public GameState GetGameState(string userName)
        {
            if (userName == null || !groupsByUserName.ContainsKey(userName))
            {
                return null;
            }

            string groupName = groupsByUserName[userName];

            if (!gamesByGroupName.ContainsKey(groupName))
            {
                return null;
            }

            return gamesByGroupName[groupName];
        }

        public GameState GetGameStateByGroupName(string groupName)
        {
            if (groupName == null || !gamesByGroupName.ContainsKey(groupName))
            {
                return null;
            }

            return gamesByGroupName[groupName];
        }

        public string GetGroupName(string userName)
        {
            if (userName == null || !groupsByUserName.ContainsKey(userName))
            {
                return null;
            }

            return groupsByUserName[userName];
        }

        public IEnumerable<KeyValuePair<string, string>> GetWaitingPlayers()
        {
            return waitingConnectionIdsByUserName;
        }

        public bool IsUserAlreadyInsideGame(string userName)
        {
            return userName != null && groupsByUserName.ContainsKey(userName);
        }

        public bool IsUserAlreadyWaiting(string userName, string connectionId)
        {
            return userName != null && connectionId != null
                && waitingConnectionIdsByUserName.ContainsKey(userName)
                && waitingConnectionIdsByUserName[userName] == connectionId;
        }

        public bool IsUserInsideGroup(string userName, string groupName)
        {
            if (userName == null || !groupsByUserName.ContainsKey(userName))
            {
                return false;
            }

            return groupName == groupsByUserName[userName];
        }

        public void RemoveGameState(string groupName)
        {
            if (groupName != null)
            {
                _ = gamesByGroupName.Remove(groupName);
            }
        }

        public void RemoveUserFromGroup(string userName)
        {
            if (userName != null)
            {
                _ = groupsByUserName.Remove(userName);
            }
        }

        public void RemoveUserFromWaitingQueue(string userName)
        {
            if (userName != null)
            {
                _ = waitingConnectionIdsByUserName.Remove(userName);
            }
        }
    }
}

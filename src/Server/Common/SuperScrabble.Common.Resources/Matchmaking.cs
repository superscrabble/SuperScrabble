namespace SuperScrabble.Common.Resources
{
    public static class Matchmaking
    {
        public static class MatchmakingErrorCodes
        {
            public const string PlayersPerGameCountNotSupported = "PlayersPerGameCountNotSupported";
            public const string GameModeNotSupported = "GameModeNotSupported";

            public const string PlayerAlreadyInsideLobby = "PlayerAlreadyInsideLobby";
            public const string UnexistingInvitationCode = "UnexistingInvitationCode";
            public const string NotEnoughPlayersToStartFriendlyGame = "NotEnoughPlayersToStartFriendlyGame";
            public const string GameLobbyFull = "GameLobbyFull";
            public const string UnauthorizedToStartGame = "UnauthorizedToStartGame";

            public static class Party
            {
                public const string PartyNotFound = "PartyNotFound";
            }
        }
    }
}

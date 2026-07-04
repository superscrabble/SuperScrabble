using SuperScrabble.Services.Game.Common.BonusCellsProviders;
using SuperScrabble.Services.Game.Common.GameplayConstantsProviders;
using SuperScrabble.Services.Game.Common.TilesProviders;
using SuperScrabble.Services.Game.Models;
using SuperScrabble.Services.Game.Models.Bags;
using SuperScrabble.Services.Game.Models.Boards;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SuperScrabble.Services.Game.Tests;

public class GameStateTurnTests
{
    private static GameState CreateTwoTeamGameState()
    {
        var firstTeam = new Team();
        firstTeam.AddPlayer("PlayerOne", "connection-1");

        var secondTeam = new Team();
        secondTeam.AddPlayer("PlayerTwo", "connection-2");

        return new GameState(
            new Bag(new StandardTilesProvider()),
            new StandardBoard(new StandardBonusCellsProvider()),
            "game-id",
            new List<Team> { firstTeam, secondTeam },
            new StandardGameplayConstants(playersPerGameCount: 2, gameTimerSeconds: 90));
    }

    [Fact(Timeout = 5000)]
    public async Task NextTeam_WhenAllTeamsHaveSurrendered_Should_Return()
    {
        GameState gameState = CreateTwoTeamGameState();

        foreach (Player player in gameState.Players)
        {
            player.LeaveGame();
        }

        gameState.NextTeam();
    }

    [Fact(Timeout = 5000)]
    public async Task NextTeam_WhenAllPlayersHaveRunOutOfTime_Should_Return()
    {
        GameState gameState = CreateTwoTeamGameState();

        foreach (Player player in gameState.Players)
        {
            gameState.RemainingSecondsByUserNames[player.UserName] = 0;
        }

        gameState.NextTeam();
    }

    [Fact(Timeout = 5000)]
    public async Task NextTeam_WhenOneTeamRemains_Should_LandOnThatTeam()
    {
        GameState gameState = CreateTwoTeamGameState();

        gameState.Teams.First().Players.First().LeaveGame();
        gameState.NextTeam();

        Assert.Equal("PlayerTwo", gameState.CurrentTeam.CurrentPlayer.UserName);
    }
}

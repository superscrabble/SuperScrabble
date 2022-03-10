namespace SuperScrabble.Services.Game.Models;

using SuperScrabble.Services.Game.Common;
using SuperScrabble.Services.Game.Common.Enums;
using SuperScrabble.Services.Game.Common.GameplayConstantsProviders;

using SuperScrabble.Services.Game.Models.Bags;
using SuperScrabble.Services.Game.Models.Boards;

public class GameState
{
    private readonly List<Team> _teams = new();
    private readonly List<GameHistoryLog> _historyLogs = new();

    public GameState(
        IBag bag,
        IBoard board,
        string groupName,
        IEnumerable<Team> teams,
        IGameplayConstantsProvider gameplayConstantsProvider)
    {
        _teams.AddRange(teams);

        Bag = bag;
        Board = board;
        GameId = groupName;
        TeamIndex = 0;
        IsGameOver = false;
        GameplayConstants = gameplayConstantsProvider;
    }

    public Dictionary<string, int> RemainingSecondsByUserNames { get; } = new();

    public IGameplayConstantsProvider GameplayConstants { get; set; }

    public IReadOnlyCollection<GameHistoryLog> HistoryLogs => _historyLogs.AsReadOnly();

    public IBag Bag { get; }

    public IBoard Board { get; }

    public string GameId { get; }

    public int TeamIndex { get; private set; }

    public bool IsGameOver { get; private set; }

    public GameMode GameMode { get; set; }

    public IReadOnlyCollection<Team> Teams => _teams.AsReadOnly();

    public IReadOnlyCollection<Player> Players =>
        _teams.SelectMany(team => team.Players).ToList().AsReadOnly();

    public Team CurrentTeam => _teams[TeamIndex];

    public int TilesCount => Bag.TilesCount;

    public bool IsTileExchangePossible =>
        TilesCount >= GameplayConstants.PlayerTilesCount;

    public void EndGame()
    {
        IsGameOver = true;
    }

    public void AddHistoryLog(GameHistoryLog historyLog)
    {
        _historyLogs.Add(historyLog);
    }

    public void EndGameIfRoomIsEmptyOrAllPlayersHaveRunOutOfTime()
    {
        int remainingTeamsCount = _teams.Count(team => !team.HasSurrendered
                && HasPlayerTime(team.CurrentPlayer.UserName));

        if (remainingTeamsCount <= 1)
        {
            EndGame();
        }
    }

    public void ResetConsecutiveSkipsCount()
    {
        foreach (Team team in _teams)
        {
            team.ResetConsecutiveSkipsCount();
        }
    }

    public void NextTeam()
    {
        while (_teams.Count > 1)
        {
            TeamIndex++;

            if (TeamIndex >= _teams.Count)
            {
                TeamIndex = 0;
            }

            if (!CurrentTeam.HasSurrendered
                && HasPlayerTime(CurrentTeam.CurrentPlayer.UserName))
            {
                break;
            }
        }
    }

    public Player? GetPlayer(string userName)
    {
        foreach (Player player in _teams.SelectMany(team => team.Players))
        {
            if (player.UserName == userName)
            {
                return player;
            }
        }

        return null;
    }

    /// <summary>
    /// </summary>
    /// <param name="userName"></param>
    /// <returns>The team of user with the given username if such exists, otherwise null</returns>
    public Team? GetTeam(string userName)
    {
        return _teams.FirstOrDefault(
            team => team.Players.Any(pl => pl.UserName == userName));
    }

    public IEnumerable<string> GetUserNamesOfPlayersWhoHaveLeftTheGame()
    {
        var playersWhoHaveLeft = new List<string>();

        foreach (Team team in _teams)
        {
            playersWhoHaveLeft.AddRange(team.Players
                .Where(pl => pl.HasLeftTheGame).Select(pl => pl.UserName));
        }

        return playersWhoHaveLeft;
    }

    private bool HasPlayerTime(string userName)
    {
        if (RemainingSecondsByUserNames.Count == 0)
        {
            return true;
        }

        return RemainingSecondsByUserNames[userName] > 0;
    }
}

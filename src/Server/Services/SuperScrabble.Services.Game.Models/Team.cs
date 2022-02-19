namespace SuperScrabble.Services.Game.Models;

public class Team
{
    private readonly List<Player> _players = new();

    public Team()
    {
        PlayerIndex = 0;
        IsTurnFinished = false;
    }

    public int PlayerIndex { get; private set; }

    public bool IsTurnFinished { get; private set; }

    public Player CurrentPlayer => _players[PlayerIndex];

    /// <summary>
    /// Determines whether all players of the team have left the game
    /// </summary>
    public bool HasSurrendered => _players.All(pl => pl.HasLeftTheGame);

    public IEnumerable<Player> Players => _players.AsReadOnly();

    /// <summary>
    /// Sets the ConsecutiveSkipsCount property of all players of the team to zero
    /// </summary>
    public void ResetConsecutiveSkipsCount()
    {
        foreach (Player player in _players)
        {
            player.ConsecutiveSkipsCount = 0;
        }
    }

    public void NextPlayer()
    {
        IsTurnFinished = false;

        while (_players.Count >= 1)
        {
            PlayerIndex++;

            if (PlayerIndex >= _players.Count)
            {
                IsTurnFinished = true;
                PlayerIndex = 0;
                break;
            }

            if (!CurrentPlayer.HasLeftTheGame)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Adds a new player to the team
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="connectionId"></param>
    /// <returns>False if the player has already been added, otherwise True</returns>
    public bool AddPlayer(string userName, string connectionId)
    {
        bool isPlayerAlreadyAdded = _players.Any(
            pl => pl.UserName == userName || pl.ConnectionId == connectionId);

        if (isPlayerAlreadyAdded)
        {
            return false;
        }

        _players.Add(new Player(userName, connectionId));
        return true;
    }
}

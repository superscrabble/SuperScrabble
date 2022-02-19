using SuperScrabble.Common.Exceptions.Matchmaking;

namespace SuperScrabble.Services.Game.Models.Parties;

public abstract class Party
{
    private readonly LinkedList<Member> _members = new();

    public Party(
        Member owner,
        string id,
        string invitationCode,
        int minPlayersToStartGameCount,
        int maxAllowedPlayersCount)
    {
        Id = id;
        InvitationCode = invitationCode;
        MinPlayersToStartGameCount = minPlayersToStartGameCount;
        MaxAllowedPlayersCount = maxAllowedPlayersCount;
        AddMember(owner);
    }

    public Member? Owner => _members.FirstOrDefault();

    public string Id { get; }

    public string InvitationCode { get; set; }

    public int MinPlayersToStartGameCount { get; }

    public int MaxAllowedPlayersCount { get; }

    public IReadOnlyCollection<Member> Members => _members.ToList().AsReadOnly();

    public bool IsEmpty => _members.Count <= 0;

    public bool IsFull => _members.Count >= MaxAllowedPlayersCount;

    public bool HasEnoughPlayersToStartGame => _members.Count >= MinPlayersToStartGameCount;

    /// <summary>
    /// </summary>
    /// <param name="userNamesToExclude"></param>
    /// <returns>All connection ids except for those which are null or belong to any of the excluded members</returns>
    public IEnumerable<string> GetConnectionIds(params string[] userNamesToExclude)
    {
        bool IsMemberValid(Member member) => member.ConnectionId != null
            && !userNamesToExclude.Contains(member.UserName);

        return _members
            .Where(IsMemberValid)
            .Select(mem => mem.ConnectionId!);
    }

    public bool IsMemberInside(string userName)
    {
        return _members.Any(mem => mem.UserName == userName);
    }

    /// <summary>
    /// Adds a new member to the party
    /// </summary>
    /// <param name="member"></param>
    /// <exception cref="GamePartyFullException"></exception>
    /// <exception cref="PlayerAlreadyInsidePartyException"></exception>
    public void AddMember(Member member)
    {
        if (IsFull)
        {
            throw new GamePartyFullException();
        }

        if (IsMemberInside(member.UserName))
        {
            throw new PlayerAlreadyInsidePartyException();
        }

        _members.AddLast(member);
    }

    /// <summary>
    /// Removes the member with the given username if such exists
    /// </summary>
    /// <param name="userName"></param>
    /// <returns>True if the member was successfully removed, otherwise False</returns>
    public bool RemoveMember(string userName)
    {
        var memberToRemove = _members.FirstOrDefault(mem => mem.UserName == userName);

        if (memberToRemove == null)
        {
            return false;
        }

        _members.Remove(memberToRemove);
        return true;
    }

    public Member? GetMember(string memberUserName)
    {
        return _members.FirstOrDefault(mem => mem.UserName == memberUserName);
    }
}

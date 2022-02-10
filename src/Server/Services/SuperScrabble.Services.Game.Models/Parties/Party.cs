namespace SuperScrabble.Services.Game.Models.Parties
{
    using SuperScrabble.Common.Exceptions.Matchmaking;

    public abstract class Party
    {
        private readonly LinkedList<Member> members = new();

        public Party(
            Member owner,
            string id,
            string invitationCode,
            int minPlayersToStartGameCount,
            int maxAllowedPlayersCount)
        {
            this.Id = id;
            this.InvitationCode = invitationCode;
            this.MinPlayersToStartGameCount = minPlayersToStartGameCount;
            this.MaxAllowedPlayersCount = maxAllowedPlayersCount;
            this.AddMember(owner);
        }

        public Member? Owner => this.members.FirstOrDefault();

        public string Id { get; }

        public string InvitationCode { get; }

        public int MinPlayersToStartGameCount { get; }

        public int MaxAllowedPlayersCount { get; }

        public IReadOnlyCollection<Member> Members => this.members.ToList().AsReadOnly();

        public bool IsEmpty => this.members.Count <= 0;

        public bool IsFull => this.members.Count >= this.MaxAllowedPlayersCount;

        public bool HasEnoughPlayersToStartGame => this.members.Count >= this.MinPlayersToStartGameCount;

        public IEnumerable<string> GetConnectionIds(params string[] memberUserNamesToExclude)
        {
            return this.members
                .Where(mem => !memberUserNamesToExclude.Contains(mem.UserName))
                .Select(mem => mem.ConnectionId);
        }

        public bool IsMemberInside(string userName)
        {
            return this.members.Any(mem => mem.UserName == userName);
        }

        public void AddMember(Member member)
        {
            if (this.IsFull)
            {
                throw new GameLobbyFullException();
            }

            if (this.IsMemberInside(member.UserName))
            {
                throw new PlayerAlreadyInsidePartyException();
            }

            this.members.AddLast(member);
        }

        public bool RemoveMember(string memberUserName)
        {
            Member? memberToRemove = this.members.FirstOrDefault(mem => mem.UserName == memberUserName);

            if (memberToRemove == null)
            {
                return false;
            }

            this.members.Remove(memberToRemove);
            return true;
        }
    }
}

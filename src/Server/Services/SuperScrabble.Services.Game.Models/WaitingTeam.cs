namespace SuperScrabble.Services.Game.Models
{
    public class WaitingTeam
    {
        public WaitingTeam(IEnumerable<Member> members)
        {
            this.Members = members.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<Member> Members { get; }
    }
}

namespace SuperScrabble.Services.Game.Models
{
    public class Member
    {
        public Member(string userName, string connectionId)
        {
            this.UserName = userName;
            this.ConnectionId = connectionId;
        }

        public string UserName { get; }

        public string ConnectionId { get; set; }
    }
}

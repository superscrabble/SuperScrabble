namespace SuperScrabble.Data.Models
{
    using Microsoft.AspNetCore.Identity;

    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            this.Games = new HashSet<GameUser>();
        }

        public virtual ICollection<GameUser> Games { get; set; }
    }
}

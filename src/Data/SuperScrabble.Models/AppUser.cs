namespace SuperScrabble.Models
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Identity;

    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            this.Games = new HashSet<UserGame>();
        }

        public virtual ICollection<UserGame> Games { get; set; }
    }
}

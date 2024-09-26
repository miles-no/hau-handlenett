using Microsoft.AspNetCore.Identity;

namespace HandlenettAPI.Models
{
    public class User : IdentityUser
    {
        public string? Initials { get; set; }
    }
}

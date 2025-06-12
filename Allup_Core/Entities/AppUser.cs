

using Microsoft.AspNetCore.Identity;

namespace Allup_Core.Entities
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}

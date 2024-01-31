using Microsoft.AspNetCore.Identity;

namespace AuraTest.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

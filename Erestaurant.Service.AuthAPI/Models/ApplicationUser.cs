using Microsoft.AspNetCore.Identity;

namespace Erestaurant.Service.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}

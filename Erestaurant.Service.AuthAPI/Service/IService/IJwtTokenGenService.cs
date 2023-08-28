using Erestaurant.Service.AuthAPI.Models;

namespace Erestaurant.Service.AuthAPI.Service.IService
{
    public interface IJwtTokenGenService
    {
        string GenerateToken(ApplicationUser applicationUser);
    }
}

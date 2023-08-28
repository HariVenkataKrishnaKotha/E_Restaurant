using Erestaurant.Web.Models;

namespace Erestaurant.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginDto loginDto);
        Task<ResponseDto?> RegisterAsync(RegistrationDto registration);
        Task<ResponseDto?> AssignRoleAsync(RegistrationDto registrationDto);
    }
}

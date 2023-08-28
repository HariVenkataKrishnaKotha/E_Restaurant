using Erestaurant.Service.AuthAPI.Models.Dto;

namespace Erestaurant.Service.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationDto registrationDto);
        Task<LoginResponseDto> Login(LoginDto loginDto);
        Task<bool> AssignRole(string email, string roleName);
    }
}

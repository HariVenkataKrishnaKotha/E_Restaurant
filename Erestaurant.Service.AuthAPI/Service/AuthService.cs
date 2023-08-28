using Erestaurant.Service.AuthAPI.Models;
using Erestaurant.Service.AuthAPI.Models.Dto;
using Erestaurant.Service.AuthAPI.Service.IService;
using Erestaurant.Services.AuthAPI.Data;
using Microsoft.AspNetCore.Identity;

namespace Erestaurant.Service.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenService _jwtTokenGenService;
        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IJwtTokenGenService jwtTokenGenService)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenService = jwtTokenGenService;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            if(user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user,roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == loginDto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if(user==null || isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "" }; ;
            }
            //Generate Jwt Token here
            var token = _jwtTokenGenService.GenerateToken(user);

            UserDto userDto = new()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = ""
            };

            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationDto registrationDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationDto.Email,
                Email = registrationDto.Email,
                NormalizedEmail = registrationDto.Email.ToUpper(),
                Name = registrationDto.Name,
                PhoneNumber = registrationDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationDto.Password);
                if (result.Succeeded)
                {
                    var userReturn = _db.ApplicationUsers.First(x => x.UserName == registrationDto.Email);
                    UserDto userDto = new()
                    {
                        Email = userReturn.Email,
                        Name = userReturn.Name,
                        Id = userReturn.Id,
                        PhoneNumber = userReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {

            }
            return "There is an error";
        }
    }
}

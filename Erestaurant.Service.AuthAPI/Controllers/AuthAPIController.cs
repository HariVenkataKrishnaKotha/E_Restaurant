using Erestaurant.Service.AuthAPI.Models.Dto;
using Erestaurant.Service.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erestaurant.Service.AuthAPI.Controllers
{
    [Route("api/authAPI")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;
        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _responseDto = new();
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDto registrationDto)
        {
            var errorMessage = await _authService.Register(registrationDto);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResponse = await _authService.Login(loginDto);
            if (loginResponse.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Username or password is not correct";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponse;
            return Ok(_responseDto);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationDto registrationDto)
        {
            var assignRoleSuccess = await _authService.AssignRole(registrationDto.Email,registrationDto.Role.ToUpper());
            if (!assignRoleSuccess)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error assigning role";
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }
    }
}

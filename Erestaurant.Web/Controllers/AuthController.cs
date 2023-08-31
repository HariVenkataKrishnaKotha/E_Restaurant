using Erestaurant.Web.Models;
using Erestaurant.Web.Service.IService;
using Erestaurant.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Erestaurant.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProviderService _tokenProviderService;
        public AuthController(IAuthService authService, ITokenProviderService tokenProviderService)
        {
            _authService = authService;
            _tokenProviderService = tokenProviderService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginDto loginDto = new LoginDto();
            return View(loginDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            ResponseDto responseDto = await _authService.LoginAsync(loginDto);

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                
                await SigninUser(loginResponseDto);
                _tokenProviderService.SetToken(loginResponseDto.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = responseDto.Message;
                return View(loginDto);
            }
        }

        [HttpGet]
        public IActionResult Register() 
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=StaticDetails.RoleAdmin,Value=StaticDetails.RoleAdmin},
                new SelectListItem(){Text=StaticDetails.RoleCustomer, Value=StaticDetails.RoleCustomer}
            };
            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationDto registrationDto)
        {
            ResponseDto responseDto = await _authService.RegisterAsync(registrationDto);
            ResponseDto assignRole;

            if(responseDto != null && responseDto.IsSuccess) 
            {
                if (string.IsNullOrEmpty(registrationDto.Role))
                {
                    registrationDto.Role = StaticDetails.RoleCustomer;
                }
                    assignRole = await _authService.AssignRoleAsync(registrationDto);
                if(assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                TempData["error"] = responseDto.Message;
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=StaticDetails.RoleAdmin,Value=StaticDetails.RoleAdmin},
                new SelectListItem(){Text=StaticDetails.RoleCustomer, Value=StaticDetails.RoleCustomer}
            };
            ViewBag.RoleList = roleList;

            return View(registrationDto);

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProviderService.ClearToken();
            return RedirectToAction("Index","Home");
        }

        private async Task SigninUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim("email", jwt.Claims.FirstOrDefault(x => x.Type == "email").Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim("name", jwt.Claims.FirstOrDefault(x => x.Type == "name").Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == "email").Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
        }
    }
}

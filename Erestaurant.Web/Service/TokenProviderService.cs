using Erestaurant.Web.Service.IService;
using Erestaurant.Web.Utility;
using Newtonsoft.Json.Linq;

namespace Erestaurant.Web.Service
{
    public class TokenProviderService : ITokenProviderService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProviderService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }
        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetails.TokenCookie);
        }

        public string? GetToken()
        {
            string? token = null;
            bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticDetails.TokenCookie, out token);
            return hasToken is true ? token : null;
        }

        public void SetToken(string token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append(StaticDetails.TokenCookie, token);
        }
    }
}

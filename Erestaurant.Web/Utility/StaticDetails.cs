namespace Erestaurant.Web.Utility
{
    public class StaticDetails
    {
        public static string CouponAPIBase;
        public static string AuthAPIBase;
        public const string RoleAdmin = "Admin";
        public const string RoleCustomer = "Customer";
        public const string TokenCookie = "JwtToken";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}

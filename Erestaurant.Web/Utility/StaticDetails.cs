namespace Erestaurant.Web.Utility
{
    public class StaticDetails
    {
        public static string CouponAPIBase;
        public static string AuthAPIBase;
        public const string RoleAdmin = "Admin";
        public const string RoleCustomer = "Customer";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}

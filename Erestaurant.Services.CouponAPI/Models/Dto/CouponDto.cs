using System.ComponentModel.DataAnnotations;

namespace Erestaurant.Services.CouponAPI.Models.Dto
{
    public class CouponDto
    {
        public class Coupon
        {
            public int CouponId { get; set; }
            public string CouponCode { get; set; }
            public double DiscountAmount { get; set; }
            public int MinAmount { get; set; }
        }
    }
}

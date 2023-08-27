using AutoMapper;
using Erestaurant.Services.CouponAPI.Models;
using Erestaurant.Services.CouponAPI.Models.Dto;

namespace Erestaurant.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>();
                config.CreateMap<Coupon, CouponDto>();
            });
            return mappingConfig;
        }
    }
}

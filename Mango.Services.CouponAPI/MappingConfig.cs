using AutoMapper;
using Mango.Services.CouponAPI.Models.DTOs;
using Mango.Services.CouponAPI.Models.Entities;

internal class MappingConfig
{
    internal static MapperConfiguration RegisterMaps()
    {
        MapperConfiguration mapperConfiguration = new(config =>
        {
            config.CreateMap<Coupon, CouponDto>().ReverseMap();
        });

        return mapperConfiguration;
    }
}
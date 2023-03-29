using AutoMapper;
using Mango.Services.CouponAPI.DbContexts;
using Mango.Services.CouponAPI.Models.DTOs;
using Mango.Services.CouponAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public CouponRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            Coupon coupon = await _applicationDbContext.Coupons.SingleAsync(s => s.CouponCode.Equals(couponCode));

            CouponDto couponDto = _mapper.Map<CouponDto>(coupon);

            return couponDto;
        }
    }
}
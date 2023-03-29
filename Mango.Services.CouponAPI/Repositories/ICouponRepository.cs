using Mango.Services.CouponAPI.Models.DTOs;

namespace Mango.Services.CouponAPI.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCode(string couponCode);
    }
}
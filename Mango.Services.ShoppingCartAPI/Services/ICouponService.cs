using Mango.Services.ShoppingCartAPI.Models.DTOs;

namespace Mango.Services.ShoppingCartAPI.Services
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
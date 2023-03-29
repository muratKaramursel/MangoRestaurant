using Mango.Web.Models.DTOs;

namespace Mango.Web.Services.IServices
{
    public interface ICouponService
    {
        Task<BaseResponseDto<CouponDto>> GetCoupon(string couponCode, string accessToken);
    }
}
using Mango.Web.Models;
using Mango.Web.Models.DTOs;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class CouponService : BaseService, ICouponService
    {
        public CouponService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<BaseResponseDto<CouponDto>> GetCoupon(string couponCode, string accessToken)
        {
            return await SendAsync<CouponDto>(
                new ApiRequest()
                {
                    Url = $"{SD.CouponAPIBase}/api/coupon/{couponCode}",
                    ApiType = Models.Enums.ApiTypes.GET,
                    Data = null,
                    AccessToken = accessToken
                }
            );
        }
    }
}
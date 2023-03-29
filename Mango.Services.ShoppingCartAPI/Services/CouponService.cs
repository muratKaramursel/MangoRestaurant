using Mango.Services.ShoppingCartAPI.Models.DTOs;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly HttpClient _httpClient;

        public CouponService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CouponDto> GetCoupon(string couponCode)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"api/coupon/{couponCode}");

            string content = await httpResponseMessage.Content.ReadAsStringAsync();

            BaseResponseDto<CouponDto> baseResponseDto = JsonConvert.DeserializeObject<BaseResponseDto<CouponDto>>(content);

            if (baseResponseDto != null && baseResponseDto.IsSuccess && baseResponseDto.ReturnObject != null)
                return baseResponseDto.ReturnObject;

            return new CouponDto();
        }
    }
}
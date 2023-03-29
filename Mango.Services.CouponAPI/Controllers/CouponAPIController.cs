using Mango.Services.CouponAPI.Models.DTOs;
using Mango.Services.CouponAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;

        public CouponAPIController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        [HttpGet]
        [Route("{couponCode}")]
        public async Task<object> Get(string couponCode)
        {
            BaseResponseDto<CouponDto> baseResponseDto = new();

            try
            {
                CouponDto couponDto = await _couponRepository.GetCouponByCode(couponCode);

                baseResponseDto.IsSuccess = true;
                baseResponseDto.ReturnObject = couponDto;
            }
            catch (Exception exception)
            {
                baseResponseDto.Messages = new()
                {
                    exception.Message,
                    exception.GetBaseException().Message
                };

                throw;
            }

            return baseResponseDto;
        }

    }
}
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Messaging;
using Mango.Services.ShoppingCartAPI.Models.DTOs;
using Mango.Services.ShoppingCartAPI.Models.Messages;
using Mango.Services.ShoppingCartAPI.Repositories;
using Mango.Services.ShoppingCartAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMessageBus _messageBus;
        private readonly ICouponService _couponService;
        private readonly IRabbitMQCartMessageSender _rabbitMQCartMessageSender;

        public ShoppingCartAPIController(ICartRepository cartRepository, IMessageBus messageBus, ICouponService couponService, IRabbitMQCartMessageSender rabbitMQCartMessageSender)
        {
            _cartRepository = cartRepository;
            _messageBus = messageBus;
            _couponService = couponService;
            _rabbitMQCartMessageSender = rabbitMQCartMessageSender;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            BaseResponseDto<CartDto> baseResponseDto = new();

            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUser(userId);
                baseResponseDto.ReturnObject = cartDto;
                baseResponseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                baseResponseDto.IsSuccess = false;
                baseResponseDto.Messages = new List<string>() { ex.ToString() };
            }

            return baseResponseDto;
        }

        [HttpPost("AddCart")]
        [Authorize]
        public async Task<object> AddCart(CartDto cartDto)
        {
            BaseResponseDto<CartDto> baseResponseDto = new();

            try
            {
                CartDto cartDt = await _cartRepository.CreateUpdateCart(cartDto);
                baseResponseDto.ReturnObject = cartDt;
                baseResponseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                baseResponseDto.IsSuccess = false;
                baseResponseDto.Messages = new List<string>() { ex.ToString() };
            }

            return baseResponseDto;
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart(CartDto cartDto)
        {
            BaseResponseDto<CartDto> baseResponseDto = new();

            try
            {
                CartDto cartDt = await _cartRepository.CreateUpdateCart(cartDto);
                baseResponseDto.ReturnObject = cartDt;
                baseResponseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                baseResponseDto.IsSuccess = false;
                baseResponseDto.Messages = new List<string>() { ex.ToString() };
            }
            return baseResponseDto;
        }

        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody] int cartId)
        {
            BaseResponseDto<bool> baseResponseDto = new();

            try
            {
                bool isSuccess = await _cartRepository.RemoveFromCart(cartId);
                baseResponseDto.ReturnObject = isSuccess;
                baseResponseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                baseResponseDto.IsSuccess = false;
                baseResponseDto.Messages = new List<string>() { ex.ToString() };
            }

            return baseResponseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            BaseResponseDto<bool> baseResponseDto = new();

            try
            {
                bool isSuccess = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                baseResponseDto.ReturnObject = isSuccess;
                baseResponseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                baseResponseDto.IsSuccess = false;
                baseResponseDto.Messages = new List<string>() { ex.ToString() };
            }

            return baseResponseDto;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            BaseResponseDto<bool> baseResponseDto = new();

            try
            {
                bool isSuccess = await _cartRepository.RemoveCoupon(userId);
                baseResponseDto.ReturnObject = isSuccess;
                baseResponseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                baseResponseDto.IsSuccess = false;
                baseResponseDto.Messages = new List<string>() { ex.ToString() };
            }

            return baseResponseDto;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout([FromBody] CheckoutHeaderDto checkoutHeader)
        {
            BaseResponseDto<bool> baseResponseDto = new();

            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUser(checkoutHeader.UserId);

                if (cartDto == null || cartDto.CartHeader == null)
                    return BadRequest();

                checkoutHeader.CartDetails = cartDto.CartDetails;

                if (!string.IsNullOrWhiteSpace(checkoutHeader.CouponCode))
                {
                    CouponDto couponDto = await _couponService.GetCoupon(checkoutHeader.CouponCode);

                    if (checkoutHeader.DiscountTotal != couponDto.DiscountAmount)
                    {
                        baseResponseDto.IsSuccess = false;
                        baseResponseDto.Messages = new List<string> { "Coupon Price has changed, please confirm." };
                        return baseResponseDto;
                    }
                }

                baseResponseDto.ReturnObject = true;
                baseResponseDto.IsSuccess = true;

                //Publish-Send with Azure
                //await _messageBus.Publish(checkoutHeader, "checkoutmessagequeue");

                //Publish-Send with RabbitMQ
                _rabbitMQCartMessageSender.SendMessage(checkoutHeader, "checkoutmessagequeue");

                await _cartRepository.ClearCart(checkoutHeader.UserId);
            }
            catch (Exception ex)
            {
                baseResponseDto.IsSuccess = false;
                baseResponseDto.Messages = new List<string>() { ex.ToString() };
            }

            return baseResponseDto;
        }

    }
}
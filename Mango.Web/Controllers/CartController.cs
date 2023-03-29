using Mango.Web.Models.DTOs;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        public async Task<IActionResult> CartIndex()
        {
            CartDto cartDto = await LoadCartDtoBasedOnLoggedInUser();

            return View(cartDto);
        }

        public async Task<IActionResult> Remove(int cartDetailId)
        {
            string userId = User.Claims.FirstOrDefault(s => s.Type == "sub")?.Value;
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<bool> baseResponseDto = await _cartService.RemoveFromCartAsync(cartDetailId, accessToken);

            if (baseResponseDto != null && baseResponseDto.IsSuccess && baseResponseDto.ReturnObject == true)
                return RedirectToAction(nameof(CartIndex));

            return View();
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<bool> baseResponseDto = await _cartService.ApplyCoupon(cartDto, accessToken);

            if (baseResponseDto != null && baseResponseDto.IsSuccess && baseResponseDto.ReturnObject == true)
                return RedirectToAction(nameof(CartIndex));

            return View();
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<bool> baseResponseDto = await _cartService.RemoveCoupon(cartDto.CartHeader.UserId, accessToken);

            if (baseResponseDto != null && baseResponseDto.IsSuccess && baseResponseDto.ReturnObject == true)
                return RedirectToAction(nameof(CartIndex));

            return View();
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            string userId = User.Claims.FirstOrDefault(s => s.Type == "sub")?.Value;
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<CartDto> baseResponseDto = await _cartService.GetCartByUserIdAsnyc(userId, accessToken);

            if (baseResponseDto == null || !baseResponseDto.IsSuccess || baseResponseDto.ReturnObject == null)
                return null;

            CartDto cartDto = baseResponseDto.ReturnObject;

            if (cartDto.CartHeader != null)
            {
                if (!string.IsNullOrWhiteSpace(cartDto.CartHeader.CouponCode))
                {
                    BaseResponseDto<CouponDto> couponBaseResponseDto = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode, accessToken);

                    if (couponBaseResponseDto != null && couponBaseResponseDto.IsSuccess && couponBaseResponseDto.ReturnObject != null)
                    {
                        CouponDto couponDto = couponBaseResponseDto.ReturnObject;
                        cartDto.CartHeader.DiscountTotal = couponDto.DiscountAmount;
                        cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
                    }
                }

                if (cartDto.CartDetails != null && cartDto.CartDetails.Any())
                    cartDto.CartHeader.OrderTotal += cartDto.CartDetails.Select(s => s.Count * s.Product.Price).Sum();
            }

            return baseResponseDto.ReturnObject;
        }

        public async Task<IActionResult> Checkout()
        {
            CartDto cartDto = await LoadCartDtoBasedOnLoggedInUser();

            return View(cartDto);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<bool> baseResponseDto = await _cartService.Checkout(cartDto.CartHeader, accessToken);

            if (baseResponseDto == null || !baseResponseDto.IsSuccess || baseResponseDto.ReturnObject == false)
            {
                if (baseResponseDto.Messages == null || !baseResponseDto.Messages.Any())
                    baseResponseDto.Messages = new() { "Error Occured" };

                TempData["Error"] = string.Join(Environment.NewLine, baseResponseDto.Messages);

                return RedirectToAction(nameof(Checkout));
            }

            return RedirectToAction(nameof(Confirmation));
        }

        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
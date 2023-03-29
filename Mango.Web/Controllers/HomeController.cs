using Mango.Web.Models;
using Mango.Web.Models.DTOs;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<List<ProductDto>> baseResponseDto = await _productService.GetProducts(accessToken);

            if (baseResponseDto != null && baseResponseDto.IsSuccess && baseResponseDto.ReturnObject != null)
                return View(baseResponseDto.ReturnObject);

            return RedirectToAction(actionName: "Error", controllerName: "Home");
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<ProductDto> baseResponseDto = await _productService.GetProduct(productId, accessToken);

            if (baseResponseDto == null || !baseResponseDto.IsSuccess)
                return RedirectToAction(actionName: "Index", controllerName: "Home");

            if (baseResponseDto.ReturnObject == null)
                return NotFound();

            return View(baseResponseDto.ReturnObject);
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> Details(ProductDto productDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            CartDto cartDto = new()
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartDetailDto cartDetail = new()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };

            var resp = await _productService.GetProduct(productDto.ProductId, accessToken);
            if (resp != null && resp.IsSuccess)
            {
                cartDetail.Product = resp.ReturnObject;
            }
            List<CartDetailDto> cartDetailsDtos = new()
            {
                cartDetail
            };
            cartDto.CartDetails = cartDetailsDtos;


            BaseResponseDto<CartDto> addToCartResp = await _cartService.AddToCartAsync(cartDto, accessToken);
            if (addToCartResp != null && addToCartResp.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(productDto);
        }


        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
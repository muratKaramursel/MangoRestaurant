using Mango.Web.Models.DTOs;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<List<ProductDto>> baseResponseDto = await _productService.GetProducts(accessToken);

            if (baseResponseDto != null || baseResponseDto.IsSuccess)
                return View(baseResponseDto.ReturnObject);

            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return View(productDto);

            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<ProductDto> baseResponseDto = await _productService.CreateProduct(productDto, accessToken);

            if (baseResponseDto == null || !baseResponseDto.IsSuccess)
                return View(productDto);

            return RedirectToAction(actionName: "Index", controllerName: "Products");
        }

        public async Task<IActionResult> Edit(int productId)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<ProductDto> baseResponseDto = await _productService.GetProduct(productId, accessToken);

            if (baseResponseDto == null || !baseResponseDto.IsSuccess)
                return RedirectToAction(actionName: "Index", controllerName: "Products");

            if (baseResponseDto.ReturnObject == null)
                return NotFound();

            return View(baseResponseDto.ReturnObject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return View(productDto);

            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<ProductDto> baseResponseDto = await _productService.UpdateProduct(productDto, accessToken);

            if (baseResponseDto == null || !baseResponseDto.IsSuccess)
                return View(productDto);

            return RedirectToAction(actionName: "Index", controllerName: "Products");
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int productId)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<ProductDto> baseResponseDto = await _productService.GetProduct(productId, accessToken);

            if (baseResponseDto == null || !baseResponseDto.IsSuccess)
                return RedirectToAction(actionName: "Index", controllerName: "Products");

            if (baseResponseDto.ReturnObject == null)
                return NotFound();

            return View(baseResponseDto.ReturnObject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(ProductDto productDto)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<ProductDto> baseResponseDto = await _productService.DeleteProduct(productDto.ProductId, accessToken);

            if (baseResponseDto == null || !baseResponseDto.IsSuccess)
                return View(productDto);

            return RedirectToAction(actionName: "Index", controllerName: "Products");
        }
    }
}
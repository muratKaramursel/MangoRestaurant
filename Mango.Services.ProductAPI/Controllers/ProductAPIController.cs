using Mango.Services.ProductAPI.Models.DTOs;
using Mango.Services.ProductAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductAPIController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            BaseResponseDto<IEnumerable<ProductDto>> baseResponseDto = new();

            try
            {
                IEnumerable<ProductDto> productDtoList = await _productRepository.GetProducts();

                baseResponseDto.IsSuccess = true;
                baseResponseDto.ReturnObject = productDtoList;
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

        [HttpGet]
        [Route("{id}")]
        public async Task<object> Get(int id)
        {
            BaseResponseDto<ProductDto> baseResponseDto = new();

            try
            {
                ProductDto productDto = await _productRepository.GetProductById(id);

                baseResponseDto.IsSuccess = true;
                baseResponseDto.ReturnObject = productDto;
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

        [HttpPost]
        [Authorize]
        public async Task<object> Post([FromBody] ProductDto productDto)
        {
            BaseResponseDto<ProductDto> baseResponseDto = new();

            try
            {
                baseResponseDto.IsSuccess = true;
                baseResponseDto.ReturnObject = await _productRepository.CreateUpdateProduct(productDto);
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

        [HttpPut]
        [Authorize]
        public async Task<object> Put([FromBody] ProductDto productDto)
        {
            BaseResponseDto<ProductDto> baseResponseDto = new();

            try
            {
                baseResponseDto.IsSuccess = true;
                baseResponseDto.ReturnObject = await _productRepository.CreateUpdateProduct(productDto);
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<object> Delete(int id)
        {
            BaseResponseDto<ProductDto> baseResponseDto = new();

            try
            {
                baseResponseDto.IsSuccess = await _productRepository.DeleteProduct(id);
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
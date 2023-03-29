using Mango.Web.Models;
using Mango.Web.Models.DTOs;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        public ProductService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<BaseResponseDto<ProductDto>> CreateProduct(ProductDto productDto, string accessToken)
        {
            return await SendAsync<ProductDto>(
                new ApiRequest()
                {
                    Url = $"{SD.ProductAPIBase}/api/products",
                    ApiType = Models.Enums.ApiTypes.POST,
                    Data = productDto,
                    AccessToken = accessToken
                }
            );
        }

        public async Task<BaseResponseDto<ProductDto>> DeleteProduct(int id, string accessToken)
        {
            return await SendAsync<ProductDto>(
                new ApiRequest()
                {
                    Url = $"{SD.ProductAPIBase}/api/products/{id}",
                    ApiType = Models.Enums.ApiTypes.DELETE,
                    Data = null,
                    AccessToken = accessToken
                }
            );
        }

        public async Task<BaseResponseDto<ProductDto>> GetProduct(int id, string accessToken)
        {
            return await SendAsync<ProductDto>(
                new ApiRequest()
                {
                    Url = $"{SD.ProductAPIBase}/api/products/{id}",
                    ApiType = Models.Enums.ApiTypes.GET,
                    Data = null,
                    AccessToken = accessToken
                }
            );
        }

        public async Task<BaseResponseDto<List<ProductDto>>> GetProducts(string accessToken)
        {
            return await SendAsync<List<ProductDto>>(
                new ApiRequest()
                {
                    Url = $"{SD.ProductAPIBase}/api/products",
                    ApiType = Models.Enums.ApiTypes.GET,
                    Data = null,
                    AccessToken = accessToken
                }
            );
        }

        public async Task<BaseResponseDto<ProductDto>> UpdateProduct(ProductDto productDto, string accessToken)
        {
            return await SendAsync<ProductDto>(
                new ApiRequest()
                {
                    Url = $"{SD.ProductAPIBase}/api/products",
                    ApiType = Models.Enums.ApiTypes.PUT,
                    Data = productDto,
                    AccessToken = accessToken
                }
            );
        }
    }
}
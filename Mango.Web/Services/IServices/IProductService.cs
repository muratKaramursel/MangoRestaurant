using Mango.Web.Models.DTOs;

namespace Mango.Web.Services.IServices
{
    public interface IProductService
    {
        Task<BaseResponseDto<List<ProductDto>>> GetProducts(string accessToken);
        Task<BaseResponseDto<ProductDto>> GetProduct(int id, string accessToken);
        Task<BaseResponseDto<ProductDto>> CreateProduct(ProductDto productDto, string accessToken);
        Task<BaseResponseDto<ProductDto>> UpdateProduct(ProductDto productDto, string accessToken);
        Task<BaseResponseDto<ProductDto>> DeleteProduct(int id, string accessToken);
    }
}
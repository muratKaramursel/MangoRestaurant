using Mango.Services.ProductAPI.Models.DTOs;

namespace Mango.Services.ProductAPI.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetProducts();
        Task<ProductDto> GetProductById(int id);
        Task<ProductDto> CreateUpdateProduct(ProductDto productDto);
        Task<bool> DeleteProduct(int id);
    }
}
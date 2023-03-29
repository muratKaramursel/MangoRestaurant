using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Models.DTOs;
using Mango.Services.ProductAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            Product product = _mapper.Map<Product>(productDto);

            try
            {
                if (product.ProductId == 0)
                    _applicationDbContext.Products.Add(product);
                else
                    _applicationDbContext.Products.Update(product);

                await _applicationDbContext.SaveChangesAsync();

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception)
            {
                throw;

            }
        }

        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                Product product = await _applicationDbContext.Products.SingleOrDefaultAsync(s => s.ProductId == id) ?? throw new Exception("NotFound");

                _applicationDbContext.Products.Remove(product);

                await _applicationDbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ProductDto> GetProductById(int id)
        {
            Product product = await _applicationDbContext.Products.SingleOrDefaultAsync(s => s.ProductId == id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            List<Product> products = await _applicationDbContext.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}
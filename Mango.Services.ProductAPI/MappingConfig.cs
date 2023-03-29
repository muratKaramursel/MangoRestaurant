using AutoMapper;
using Mango.Services.ProductAPI.Models.DTOs;
using Mango.Services.ProductAPI.Models.Entities;

namespace Mango.Services.ProductAPI
{
    internal class MappingConfig
    {
        internal static MapperConfiguration RegisterMaps()
        {
            MapperConfiguration mapperConfiguration = new(config =>
            {
                config.CreateMap<ProductDto, Product>();
                config.CreateMap<Product, ProductDto>();
            });

            return mapperConfiguration;
        }
    }
}
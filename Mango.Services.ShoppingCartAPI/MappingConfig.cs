using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTOs;
using Mango.Services.ShoppingCartAPI.Models.Entities;

namespace Mango.Services.ShoppingCartAPI
{
    internal class MappingConfig
    {
        internal static MapperConfiguration RegisterMaps()
        {
            MapperConfiguration mapperConfiguration = new(config =>
            {
                config.CreateMap<Product, ProductDto>().ReverseMap();
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                config.CreateMap<CartDetail, CartDetailDto>().ReverseMap();

                config.CreateMap<Cart, CartDto>().ReverseMap();
            });

            return mapperConfiguration;
        }
    }
}
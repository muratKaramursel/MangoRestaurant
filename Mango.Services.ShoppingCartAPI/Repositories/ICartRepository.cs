using Mango.Services.ShoppingCartAPI.Models.DTOs;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUser(string userId);
        Task<CartDto> CreateUpdateCart(CartDto cartDto);
        Task<bool> ClearCart(string userId);
        Task<bool> RemoveFromCart(int cartDetailId);
        Task<bool> ApplyCoupon(string userId, string couponCode);
        Task<bool> RemoveCoupon(string userId);
    }
}
using Mango.Web.Models.DTOs;

namespace Mango.Web.Services.IServices
{
    public interface ICartService
    {
        Task<BaseResponseDto<CartDto>> GetCartByUserIdAsnyc(string userId, string token = null);
        Task<BaseResponseDto<CartDto>> AddToCartAsync(CartDto cartDto, string token = null);
        Task<BaseResponseDto<CartDto>> UpdateCartAsync(CartDto cartDto, string token = null);
        Task<BaseResponseDto<bool>> RemoveFromCartAsync(int cartId, string token = null);
        Task<BaseResponseDto<bool>> ApplyCoupon(CartDto cartDto, string token = null);
        Task<BaseResponseDto<bool>> RemoveCoupon(string userId, string token = null);
        Task<BaseResponseDto<bool>> Checkout(CartHeaderDto cartHeaderDto, string token = null);
    }
}
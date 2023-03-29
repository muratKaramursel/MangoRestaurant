using Mango.Web.Models;
using Mango.Web.Models.DTOs;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class CartService : BaseService, ICartService
    {
        public CartService(IHttpClientFactory clientFactory) : base(clientFactory)
        {
        }

        public async Task<BaseResponseDto<CartDto>> AddToCartAsync(CartDto cartDto, string token = null)
        {
            return await SendAsync<CartDto>(new ApiRequest()
            {
                ApiType = Models.Enums.ApiTypes.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/AddCart",
                AccessToken = token
            });
        }

        public async Task<BaseResponseDto<CartDto>> GetCartByUserIdAsnyc(string userId, string token = null)
        {
            return await SendAsync<CartDto>(new ApiRequest()
            {
                ApiType = Models.Enums.ApiTypes.GET,
                Url = SD.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId,
                AccessToken = token
            });
        }

        public async Task<BaseResponseDto<bool>> RemoveFromCartAsync(int cartId, string token = null)
        {
            return await SendAsync<bool>(new ApiRequest()
            {
                ApiType = Models.Enums.ApiTypes.POST,
                Data = cartId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart",
                AccessToken = token
            });
        }

        public async Task<BaseResponseDto<CartDto>> UpdateCartAsync(CartDto cartDto, string token = null)
        {
            return await SendAsync<CartDto>(new ApiRequest()
            {
                ApiType = Models.Enums.ApiTypes.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/UpdateCart",
                AccessToken = token
            });
        }

        public async Task<BaseResponseDto<bool>> ApplyCoupon(CartDto cartDto, string token = null)
        {
            return await SendAsync<bool>(new ApiRequest()
            {
                ApiType = Models.Enums.ApiTypes.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon",
                AccessToken = token
            });
        }

        public async Task<BaseResponseDto<bool>> RemoveCoupon(string userId, string token = null)
        {
            return await SendAsync<bool>(new ApiRequest()
            {
                ApiType = Models.Enums.ApiTypes.POST,
                Data = userId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCoupon",
                AccessToken = token
            });
        }

        public async Task<BaseResponseDto<bool>> Checkout(CartHeaderDto cartHeaderDto, string token = null)
        {
            return await SendAsync<bool>(new ApiRequest()
            {
                ApiType = Models.Enums.ApiTypes.POST,
                Data = cartHeaderDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/Checkout",
                AccessToken = token
            });
        }
    }
}
using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTOs;
using Mango.Services.ShoppingCartAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public CartRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<bool> ClearCart(string userId)
        {
            CartHeader cartHeaderFromDb = await _applicationDbContext.CartHeaders.SingleOrDefaultAsync(u => u.UserId == userId);

            if (cartHeaderFromDb == null)
                return false;

            _applicationDbContext.CartDetails.RemoveRange(_applicationDbContext.CartDetails.Where(u => u.CartHeaderId == cartHeaderFromDb.CartHeaderId));
            _applicationDbContext.CartHeaders.Remove(cartHeaderFromDb);

            await _applicationDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {

            Cart cart = _mapper.Map<Cart>(cartDto);

            //check if product exists in database, if not create it!
            var prodInDb = await _applicationDbContext.Products.SingleOrDefaultAsync(u => u.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);

            if (prodInDb == null)
            {
                _applicationDbContext.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _applicationDbContext.SaveChangesAsync();
            }


            //check if header is null
            var cartHeaderFromDb = await _applicationDbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);

            if (cartHeaderFromDb == null)
            {
                //create header and details
                _applicationDbContext.CartHeaders.Add(cart.CartHeader);
                await _applicationDbContext.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                _applicationDbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _applicationDbContext.SaveChangesAsync();
            }
            else
            {
                //if header is not null
                //check if details has same product
                var cartDetailsFromDb = await _applicationDbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                    u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                if (cartDetailsFromDb == null)
                {
                    //create details
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _applicationDbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _applicationDbContext.SaveChangesAsync();
                }
                else
                {
                    //update the count / cart details
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDb.Count;
                    cart.CartDetails.FirstOrDefault().CartDetailId = cartDetailsFromDb.CartDetailId;
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                    _applicationDbContext.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _applicationDbContext.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartDto>(cart);

        }

        public async Task<CartDto> GetCartByUser(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _applicationDbContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId)
            };

            cart.CartDetails = _applicationDbContext.CartDetails
                .Include(s => s.CartHeader)
                .Include(s => s.Product)
                .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId);

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveFromCart(int cartDetailId)
        {
            try
            {
                CartDetail cartDetail = await _applicationDbContext.CartDetails.SingleAsync(u => u.CartDetailId == cartDetailId);

                int totalCountOfCartItems = _applicationDbContext.CartDetails.Where(u => u.CartHeaderId == cartDetail.CartHeaderId).Count();

                _applicationDbContext.CartDetails.Remove(cartDetail);

                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _applicationDbContext.CartHeaders.SingleAsync(u => u.CartHeaderId == cartDetail.CartHeaderId);

                    _applicationDbContext.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _applicationDbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            CartHeader cartHeader = await _applicationDbContext.CartHeaders.SingleAsync(s => s.UserId.Equals(userId));
            cartHeader.CouponCode = couponCode;
            _applicationDbContext.CartHeaders.Update(cartHeader);
            await _applicationDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            CartHeader cartHeader = await _applicationDbContext.CartHeaders.SingleAsync(s => s.UserId.Equals(userId));
            cartHeader.CouponCode = null;
            _applicationDbContext.CartHeaders.Update(cartHeader);
            await _applicationDbContext.SaveChangesAsync();

            return true;
        }
    }
}
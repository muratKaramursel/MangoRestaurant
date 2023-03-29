namespace Mango.Services.ShoppingCartAPI.Models.DTOs
{
    public class CartDetailDto
    {
        public int CartDetailId { get; set; }
        public int CartHeaderId { get; set; }
        public int ProductId { get; set; }        
        public int Count { get; set; }
        public CartHeaderDto CartHeader { get; set; }
        public ProductDto Product { get; set; }
    }
}
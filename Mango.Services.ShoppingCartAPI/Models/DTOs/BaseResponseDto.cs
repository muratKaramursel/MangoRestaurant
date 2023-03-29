namespace Mango.Services.ShoppingCartAPI.Models.DTOs
{
    public class BaseResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }
        public T ReturnObject { get; set; }
    }
}
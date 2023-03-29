namespace Mango.Services.CouponAPI.Models.DTOs
{
    public class BaseResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }
        public T ReturnObject { get; set; }
    }
}
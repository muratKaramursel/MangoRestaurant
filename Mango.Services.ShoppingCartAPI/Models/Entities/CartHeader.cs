using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCartAPI.Models.Entities
{
    public class CartHeader
    {
        #region Properties
        [Key]
        public int CartHeaderId { get; set; }
        [Required]
        public string UserId { get; set; }
        public string CouponCode { get; set; }
        #endregion Properties
    }
}
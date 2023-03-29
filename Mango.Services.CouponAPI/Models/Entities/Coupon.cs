using System.ComponentModel.DataAnnotations;

namespace Mango.Services.CouponAPI.Models.Entities
{
    public class Coupon
    {
        #region Properties
        [Key]
        public int CouponId { get; set; }

        [Required]
        public string CouponCode { get; set; }

        public double DiscountAmount { get; set; }
        #endregion Properties
    }
}
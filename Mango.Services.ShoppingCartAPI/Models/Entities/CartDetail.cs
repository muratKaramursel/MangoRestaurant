using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models.Entities
{
    public class CartDetail
    {
        #region Properties
        [Key]
        public int CartDetailId { get; set; }
        public int CartHeaderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        #endregion Properties

        #region ForeignKeys
        [ForeignKey("CartHeaderId")]
        public virtual CartHeader CartHeader { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        #endregion ForeignKeys
    }
}
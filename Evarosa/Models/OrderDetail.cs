using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

		public int ProductId { get; set; }

        public int? SkuId { get; set; }

        public int Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal UnitPrice { get; set; } = decimal.Zero;

        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal Amount
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public virtual Sku Sku { get; set; }
    }
}
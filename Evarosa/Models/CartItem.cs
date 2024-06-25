using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class CartItem
    {
        [Key]
        public string RecordId { get; set; }

        public string CartId { get; set; }

        public int ProductId { get; set; }

        public int? SkuId { get; set; }

        [Display(Name = "Số lượng")]
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int Quantity { get; set; }

        [Display(Name = "Đơn giá")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal Price { get; set; } = decimal.Zero;

        [Display(Name = "Thành tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal Total
        {
            get
            {
                return Quantity * Price;
            }
        }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public virtual Product Product { get; set; }
        public virtual Sku Sku { get; set; }
    }

}
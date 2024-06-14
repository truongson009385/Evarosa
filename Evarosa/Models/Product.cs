using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evarosa.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Mã sản phẩm")]
        public string? MaSP { set; get; }

        [Display(Name = "Danh mục sản phẩm")]
        public int ProductCategoryId { get; set; }

        [Display(Name = "Tên sản phẩm"), StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự.")]
        public string Name { get; set; }

        [Display(Name = "Mô tả ngắn")]
        public string? ShortDescription { set; get; }

        [Display(Name = "Thẻ title"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string? Title { get; set; }

        [Display(Name = "Thẻ description"), StringLength(int.MaxValue, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string? Description { get; set; }

        [Display(Name = "Thông tin chi tiết")]
        public string? Content { get; set; }

        [Display(Name = "URL"), StringLength(255, ErrorMessage = "URL không được vượt quá 255 ký tự.")]
        public string Url { get; set; }

        [Display(Name = "Ảnh sản phẩm")]
        public string? Images { get; set; }

        [Display(Name = "Tồn kho")]
        public int InStock { get; set; } = 0;

        [Display(Name = "Giá bán"), DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal Price { get; set; } = decimal.Zero;

        [Display(Name = "Giá khuyến mãi"), DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal PriceSale { get; set; } = decimal.Zero;

        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal FinalPrice
        {
            get
            {
                return PriceSale != decimal.Zero ? PriceSale : Price;
            }
        }

        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal SavePrice
        {
            get
            {
                if (Price == 0 || PriceSale == 0 || PriceSale >= Price)
                {
                    return decimal.Zero;
                }
                return Price - PriceSale;
            }
        }

        public decimal Percent
        {
            get
            {
                if (Price == 0 || PriceSale == 0)
                {
                    return decimal.Zero;
                }

                decimal percent = (Price - PriceSale) / Price * 100;
                return Math.Round(percent, 0);
            }
        }

        [Display(Name = "Hoạt đông")]
        public bool Active { get; set; } = true;

        [Display(Name = "Hiển thị ở sản phẩm nổi bật")]
        public bool ShowOutstanding { set; get; } = false;

        [Display(Name = "Hiện thị trang chủ")]
        public bool ShowHome { set; get; } = false;


        [Display(Name = "Số thứ tự"), Required(ErrorMessage = "Bạn chưa nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; } = 1;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ảnh đại diện")]
        public string? Avatar
        {
            get
            {
                return Images?.Split(',')[0];
            }
        }

        [Display(Name = "Danh sách ảnh")]
        public string[] ArrImage
        {
            get
            {
                return Images?.Split(',') ?? [];
            }
        }

        [Display(Name = "Danh mục sản phẩm")]
        public virtual ProductCategory ProductCategory { get; set; }
    }

    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Danh mục cha")]
        public int? ParentCategoryId { get; set; }

        [Required(ErrorMessage = "Phải có tên danh mục")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [Display(Name = "Tên danh mục")]
        public string Title { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Mô tả ngắn")]
        public string? Description { set; get; }

        [Display(Name = "Ảnh")]
        public string? Image { set; get; }

        [Display(Name = "Số thứ tự"), Required(ErrorMessage = "Bạn chưa nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; } = 1;

        [DataType(DataType.Text)]
        [Display(Name = "Nội dung danh mục")]
        public string? Content { set; get; }

        [Display(Name = "Hiển thị ở trang chủ")]
        public bool ShowHome { set; get; } = false;

        [Display(Name = "Hiển thị ở bộ lọc")]
        public bool ShowMenu { set; get; } = false;

        [Display(Name = "Hiển thị ở header")]
        public bool ShowHeader { set; get; } = false;

        [Display(Name = "Hiển thị ở sản phẩm nổi bật")]
        public bool ShowOutstanding { set; get; } = false;

        [Display(Name = "Trưng bày sản phẩm ở trang chủ")]
        public bool Display { set; get; } = false;

        [Display(Name = "Hoạt động")]
        public bool Active { set; get; } = true;

        [Required(ErrorMessage = "Phải tạo url")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        [Display(Name = "Url hiện thị")]
        public string Url { set; get; }

        public ICollection<ProductCategory> CategoryChildren { get; set; }

        [ForeignKey("ParentCategoryId")]
        [Display(Name = "Danh mục cha")]
        public ProductCategory ParentCategory { set; get; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
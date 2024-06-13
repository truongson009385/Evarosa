using Evarosa.Models;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace Evarosa.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<int> CategoryIds { get; set; }
        public int? InfoId { get; set; }
        public Product Product { get; set; }
        public string? Url { get; set; }
        [Display(Name = "Giá niêm yết"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public string? Price { get; set; }
        [Display(Name = "Giá khuyến mãi"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public string? PriceSale { get; set; }
        public IPagedList<Product> ListProduct { get; set; }
        
        public string Term { get; set; }
        public int? CategoryId { get; set; }
        public int? InStock { get; set; }
        public string? MaSP { get; set; }
        public DateTime? Timer { get; set; }

    }

    public class ProductCategoryViewModel
    {
        public string Term { get; set; }
        public string? Url { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public IPagedList<ProductCategory> ListProductCategory { get; set; }
    }
}

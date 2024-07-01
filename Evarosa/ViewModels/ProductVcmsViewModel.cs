using Evarosa.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace Evarosa.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<int> CategoryIds { get; set; }
        public int? InfoId { get; set; }
        public string? Url { get; set; }
        [Display(Name = "Giá niêm yết"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public string? Price { get; set; }
        [Display(Name = "Giá khuyến mãi"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public string? PriceSale { get; set; }
        public Product Product { get; set; }
        public IPagedList<Product> ListProduct { get; set; }
        public IEnumerable<Option> Options { get; set; }
        public string Term { get; set; }
        public int? CategoryId { get; set; }
        public int? InStock { get; set; }
        public string? MaSP { get; set; }
        public DateTime? Timer { get; set; }
        public string[] Images { get; set; }
        public SkuProductViewModel SkuProduct { get; set; }
        public OptionProductVcms? OptionProductVcms { get; set; }

    }

    public class ProductCategoryViewModel
    {
        public string Image { get; set; }
        public string Term { get; set; }
        public string? Url { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public IPagedList<ProductCategory> ListProductCategory { get; set; }
    }

    public class OptionViewModel
    {
        public Option Option { get; set; }
        public IPagedList<Option> ListOption { get; set; }
        public SelectList SelectListGroup { get; set; }
    }

    public class SkuProductViewModel
    {
        public List<string> Attrs { get; set; }
        public int[] OptionIds { get; set; }
        public string[] Options { get; set; }
        public int[] Ids { get; set; }
        public string[] Values { get; set; }
        public string[] Skus { get; set; }
        public int[] Stocks { get; set; }
        public string[]? Images { get; set; }
        public string[]? Prices { get; set; }
        public string[]? PriceSales { get; set; }
        public string? FirstItem { get; set; }
        public int Indexs { get; set; } = 0;
    }

    public class OptionProductVcms
    {
        public int Id { get; set; }
        public string Options { get; set; }
    }
}

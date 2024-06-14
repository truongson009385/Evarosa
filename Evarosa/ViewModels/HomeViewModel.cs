using Evarosa.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace Evarosa.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<ProductCategory> DisplayCategories { get; set; } = new List<ProductCategory>();
        public IEnumerable<ProductCategory> ProductOutstanding { get; set; } = new List<ProductCategory>();
        public IEnumerable<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        public IEnumerable<Banner> Banner { get; set; } = new List<Banner>();
        public IEnumerable<Banner> Partner { get; set; } = new List<Banner>();
        public IEnumerable<Article> Articles { get; set; } = new List<Article>();
    }

    public class HeaderViewModel
    {
        public IEnumerable<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        public SelectList SelectCategories { get; set; }
        public int Count { get; set; } = 0;
        public CartMiniViewModel CartMini { get; set; } = new CartMiniViewModel();
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; } = new List<ArticleCategory>();
    }

    public class PageProductViewModel
    {
        public string Term { get; set; }
        public string Url { get; set; }
        public string MinMax { get; set; }
        public string Sort { get; set; }

        public IPagedList<Product> ListProduct { get; set; }
        public Product Product { get; set; } = new Product();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        public ProductCategory ProductCategory { get; set; } = new ProductCategory();
    }
}

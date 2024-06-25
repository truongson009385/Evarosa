using Evarosa.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
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

    public class OptionGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Option> Options { get; set; }

        public class Option
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }

    public class FilterBarViewModel
    {
        public IEnumerable<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }

    public class HeaderViewModel
    {
        public IEnumerable<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        public SelectList SelectCategories { get; set; }
        public int Count { get; set; } = 0;
        public CartMiniViewModel CartMini { get; set; } = new CartMiniViewModel();
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; } = new List<ArticleCategory>();
    }

    public class FooterViewModel
    {
        public ArticleCategory? ArticleCategory { get; set; }
    }

    public class PageProductViewModel
    {
        public string Term { get; set; }
        public string Url { get; set; }
        public string MinMax { get; set; }
        public string Sort { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal? Price { get; set; }


        public IPagedList<Product> ListProduct { get; set; }
        public Product Product { get; set; } = new Product();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        public ProductCategory ProductCategory { get; set; } = new ProductCategory();
        public IEnumerable<OptionGroup> OptionGroups { get; set; } = new List<OptionGroup>();
    }

    public class PageArticleViewModel
    {
        public IPagedList<Article> ListArticle { get; set; }
        public Article Article { get; set; } = new Article();
        public IEnumerable<Article> Articles { get; set; } = new List<Article>();
        public IEnumerable<Article> SuKien { get; set; } = new List<Article>();
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; } = new List<ArticleCategory>();
        public ArticleCategory ArticleCategory { get; set; } = new();
    }

    public class PageContactViewModel
    {
        public Contact Contact { get; set; } = new Contact();
    }
}

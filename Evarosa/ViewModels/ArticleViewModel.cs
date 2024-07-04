using Evarosa.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace Evarosa.ViewModels
{
    public class ArticleCategoryViewModel
    {
        public IPagedList<ArticleCategory> ListArticleCategory { get; set; }
        public ArticleCategory ArticleCategory { get; set; }
        public SelectList SelectListCategory { get; set; }
    }

    public class ArticleViewModel
    {
        [Display(Name = "Ngày đăng")]
        public string? CreateDate { get; set; }
        public Article Article { get; set; }
        public SelectList SelectListCategory { get; set; }
        public IPagedList<Article> ListArticle { get; set; }
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; } = new List<ArticleCategory>();

        //Bộ lọc
        public TypeArticle? TypeArticle { get; set; }
        public string? Term { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? Timer { get; set; }
        public string Image { get; set; }

    }
}

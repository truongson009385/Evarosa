using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evarosa.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Danh mục"), Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public int ArticleCategoryId { get; set; }

        [Required(ErrorMessage = "Phải có tiêu đề bài viết")]
        [Display(Name = "Tiêu đề")]
        [StringLength(160, MinimumLength = 1, ErrorMessage = "{0} dài {1} đến {2}")]
        public string Name { set; get; }

        [Display(Name = "Mô tả ngắn")]
        public string? ShortDescription { set; get; }

        [Display(Name = "Thẻ title"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string? TitleMeta { get; set; }

        [Display(Name = "Thẻ description"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string? DescriptionMeta { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? Image { set; get; }

        public string Url { set; get; }

        [Display(Name = "Nội dung")]
        public string? Body { set; get; }

        [Display(Name = "Hoạt động")]
        public bool Active { set; get; } = true;

        [Display(Name = "Hiển thị ở trang chủ")]
        public bool ShowHome { set; get; }

        [Display(Name = "Hiển thị chân trang")]
        public bool ShowFooter { get; set; }

        [Display(Name = "Số thứ tự"), Required(ErrorMessage = "Bạn chưa nhập số thứ tự"), UIHint("NumberBox"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương")]
        public int Sort { get; set; } = 1;

        public int Views { set; get; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedAt { set; get; } = DateTime.UtcNow;

        public ArticleCategory ArticleCategory { get; set; }
    }

    public class ArticleCategory
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Danh mục cha")]
        public int? ParentCategoryId { get; set; }

        [Required(ErrorMessage = "Phải có tên danh mục")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [Display(Name = "Tên danh mục")]
        public string Title { get; set; }

        [Display(Name = "Nội dung danh mục")]
        public string? Description { set; get; }

        [Display(Name = "Ảnh đại diện")]
        public string? Image { set; get; }

        [Display(Name = "Hoạt động")]
        public bool Active { set; get; } = true;

        [Display(Name = "Hiển thị header")]
        public bool ShowHeader { set; get; } = true;

        [Display(Name = "Hiển thị trang chủ")]
        public bool ShowHome { get; set; }

        [Display(Name = "Hiển thị chân trang")]
        public bool ShowFooter { get; set; }

        [Display(Name = "Url")]
        public string Url { set; get; }

        [Display(Name = "Thứ tự")]
        public int Sort { set; get; } = 1;

        public virtual ICollection<ArticleCategory> CategoryChildren { get; set; }

        [ForeignKey("ParentCategoryId")]
        [Display(Name = "Danh mục cha")]
        public virtual ArticleCategory ParentCategory { set; get; }

        public virtual ICollection<Article> Articles { get; set; }

        [Display(Name = "Loại danh mục")]
        public TypeArticle Type { get; set; }


    }

    public enum TypeArticle
    {
        [Display(Name = "Tin tức")]
        TinTuc,
        [Display(Name = "Giới thiệu")]
        GioiThieu,
    }
}

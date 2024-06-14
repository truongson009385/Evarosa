using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class ConfigSite
    {
        [Key]
        public int Id { get; set; }

        //Cấu hình
        [Display(Name = "Thẻ title"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string Title { get; set; }

        [Display(Name = "Thẻ description"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string? Description { get; set; }

        [StringLength(4000, ErrorMessage = "Tối đa 4000 ký tự"), Display(Name = "Mã Google Analytics"), UIHint("TextArea")]
        public string? GoogleAnalytics { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Tài khoản Mail"), UIHint("TextBox")]
        public string? EmailConfig { get; set; }

        [Display(Name = "Mật khẩu Mail"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string? PassWordMail { get; set; }

        //Mạng xã hội
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Facebook"),
         Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string? Facebook { get; set; }

        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn GoogleMap"),
        Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string? GoogleMapLink { get; set; }

        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn What's App"),
        Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string? WhatsApp { get; set; }

        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Viber"),
        Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string? Viber { get; set; }

        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Youtube"),
        Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string? Youtube { get; set; }

        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Twitter"),
        Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string? Twitter { get; set; }

        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Instagram"),
        Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string? Instagram { get; set; }

        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Linkedin"),
        Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string? Linkedin { get; set; }

        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Tài khoản Zalo"),
       Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string? Zalo { get; set; }

        //Liên hệ
        [Display(Name = "Hotline"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string? Hotline { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Tài khoản Email"), UIHint("TextBox")]
        public string? Email { get; set; }

        [StringLength(4000, ErrorMessage = "Tối đa 4000 ký tự"), Display(Name = "Mã nhúng Live chat"),
        UIHint("TextArea")]
        public string? LiveChat { get; set; }

        [StringLength(2000, ErrorMessage = "Tối đa 2000 ký tự"), Display(Name = "Thông tin liên hệ"), UIHint("EditorBox")]
        public string? ContactInfo { get; set; }

        //Ảnh
        [Display(Name = "Logo"), StringLength(500)]
        public string? Image { get; set; }

        [Display(Name = "Favicon"), StringLength(500)]
        public string? Favicon { get; set; }

        [StringLength(2000, ErrorMessage = "Tối đa 2000 ký tự"), Display(Name = "Thông tin chân trang"), UIHint("EditorBox")]
        public string? FooterInfo { get; set; }

        //Giới thiệu
        [Display(Name = "Tiêu đề giới thiệu"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string? AboutTitle { get; set; }
        [StringLength(2000, ErrorMessage = "Tối đa 2000 ký tự"), Display(Name = "Văn bản giới thiệu"), UIHint("EditorBox")]
        public string? AboutText { get; set; }
        [Display(Name = "Ảnh giới thiệu"), StringLength(500)]
        public string? AboutImage { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn giới thiệu"),
       Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string? AboutUrl { get; set; }

        //Địa chỉ
        [Display(Name = "Địa chỉ"), UIHint("TextBox")]
        public string? Place { get; set; }

        [StringLength(4000, ErrorMessage = "Tối đa 4000 ký tự"), Display(Name = "Mã Google Map"), UIHint("TextArea")]
        public string? GoogleMap { get; set; }
    }
}

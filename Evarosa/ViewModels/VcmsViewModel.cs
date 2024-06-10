using Evarosa.Models;
using System.ComponentModel.DataAnnotations;

namespace Evarosa.ViewModels
{
    public class AdminForm
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
    }

    public class AdminViewModel
    {
        public Admin Admin { get; set; }
        public IEnumerable<Admin> Admins { get; set; }
    }

    public class InfoAdminViewModel
    {
        public int Admins { get; set; } = 0;
        public int Articles { get; set; } = 0;
        public int Contacts { get; set; } = 0;
        public int Banners { get; set; } = 0;
    }

    public class ChangePasswordModel
    {
        [Display(Name = "Mật khẩu hiện tại"), Required(ErrorMessage = "Hãy nhập mật khẩu hiện tại"), UIHint("Password")]
        public string OldPassword { get; set; }
        [Display(Name = "Mật khẩu mới"), Required(ErrorMessage = "Hãy nhập mật khẩu mới"),
         StringLength(16, MinimumLength = 4, ErrorMessage = "Mật khẩu từ 4, 16 ký tự"), UIHint("Password")]
        public string Password { get; set; }
        [Display(Name = "Nhập lại mật khẩu"), System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Nhập lại mật khẩu không chính xác"),
         UIHint("Password")]
        public string ConfirmPassword { get; set; }
    }
}

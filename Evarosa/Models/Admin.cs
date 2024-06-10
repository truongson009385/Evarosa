using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Evarosa.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Tên đăng nhập"), Required(ErrorMessage = "Hãy nhập tài khoản")]
        public string Username { get; set; }

        [DisplayName("Mật khẩu"), Required(ErrorMessage = "Hãy nhập mật khẩu"), StringLength(60, ErrorMessage = "Tối đa 60 ký tự")]
        public string Password { get; set; }

        [Display(Name = "Hoạt động", Description = "Hoạt động")]
        public bool Active { get; set; }

        public Admin()
        {
            Active = true;
        }
    }
}

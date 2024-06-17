using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Họ và tên"), UIHint("MemberTextBox")]
        [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Họ và tên không được dài quá 100 ký tự.")]
        public string FullName { get; set; }

        [Display(Name = "Địa chỉ email"), UIHint("MemberTextBox")]
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        public string? EmailConfirmation { get; set; }

        public string? Token { get; set; }

        [Display(Name = "Số điện thoại"), UIHint("MemberTextBox")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [StringLength(10, ErrorMessage = "Tối đa 10 ký tự")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ"), UIHint("MemberTextBox")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được dài quá 200 ký tự.")]
        public string? Address { get; set; }

        [Display(Name = "Ảnh")]
        public string? Image { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Ngày sinh"), UIHint("MemberDateBox")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Hoạt động")]
        public bool Active { get; set; } = true;

        public ICollection<MemberAddress> MemberAddresses { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}

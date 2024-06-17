using Evarosa.Models;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace Evarosa.ViewModels
{
    public class MemberViewModel
    {
        public Member Member { get; set; }
    }

    public class MemberLogin
    {
        [Display(Name = "Địa chỉ email"), UIHint("MemberTextBox")]
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Display(Name = "Mật khẩu"), UIHint("MemberPassword")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public class MemberForm
    {
        [Display(Name = "Họ và tên"), UIHint("MemberTextBox")]
        [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Họ và tên không được dài quá 100 ký tự.")]
        public string FullName { get; set; }

        [Display(Name = "Email"), UIHint("MemberTextBox")]
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại"), UIHint("MemberTextBox")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng.")]
        [StringLength(10, ErrorMessage = "Tối đa 10 ký tự")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ"), UIHint("MemberTextBox")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được dài quá 200 ký tự.")]
        public string? Address { get; set; }

        [Display(Name = "Mật khẩu"), UIHint("MemberPassword")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
        [Display(Name = "Xác nhận mật khẩu"), UIHint("MemberPassword"), Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu phải khớp.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Ngày sinh"), UIHint("MemberDateBox")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Display(Name = "Email"), UIHint("MemberTextBox")]
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại"), UIHint("MemberPassword")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu"), UIHint("MemberPassword")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
        [DataType(DataType.Password), UIHint("MemberPassword")]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp!")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Display(Name = "Email"), UIHint("MemberTextBox")]
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu"), UIHint("MemberPassword")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu"), UIHint("MemberPassword")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp!")]
        public string ConfirmPassword { get; set; }

        public string? Code { get; set; } = string.Empty;
    }

    public class MemberAddressViewModel
    {
        public IEnumerable<MemberAddress> MemberAddresses { get; set; } = new List<MemberAddress>();
        public IPagedList<MemberAddress> ListMemberAddress { get; set; }

    }

    public class MemberOrderViewModel
    {
        public IPagedList<Order> ListOrder { get; set; }

    }


    public class MemberComponentViewModel
    {
        public Member Member { get; set; }

        public int Addresses { get; set; } = 0;
        public int Orders { get; set; } = 0;
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal? Total { get; set; } = decimal.Zero;


    }
}

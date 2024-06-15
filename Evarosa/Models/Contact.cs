using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Hãy nhập họ tên"), UIHint("TextBox"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string FullName { get; set; }

        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
         Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email"), Required(ErrorMessage = "Hãy nhập Email"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }

        [Display(Name = "Địa chỉ"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Address { get; set; }

        [Display(Name = "Nội dung"), Required(ErrorMessage = "Hãy nhập nội dung"), DataType(DataType.MultilineText), StringLength(4000)]
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

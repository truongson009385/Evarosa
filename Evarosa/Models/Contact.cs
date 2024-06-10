using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Display(Name = "Full Name"), Required(ErrorMessage = "Please enter your full name"), UIHint("TextBox"), StringLength(100, ErrorMessage = "Maximum 100 characters")]
        public string FullName { get; set; }

        [Display(Name = "Phone Number"), RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Invalid phone number format!"),
         Required(ErrorMessage = "Please enter your phone number"), StringLength(10, ErrorMessage = "Maximum 10 characters"), UIHint("TextBox")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email"), Required(ErrorMessage = "Please enter your email"), StringLength(100, ErrorMessage = "Maximum 100 characters"), EmailAddress(ErrorMessage = "Invalid email address"), UIHint("TextBox")]
        public string Email { get; set; }

        [Display(Name = "Address"), StringLength(200, ErrorMessage = "Maximum 200 characters")]
        public string? Address { get; set; }

        [Display(Name = "Content"), Required(ErrorMessage = "Please enter the content"), DataType(DataType.MultilineText), StringLength(4000)]
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

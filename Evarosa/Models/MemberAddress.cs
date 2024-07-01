using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class MemberAddress
    {
        [Key]
        public int Id { get; set; }

        public int MemberId { get; set; }

        [Display(Name = "Họ & Tên"),
        Required(ErrorMessage = "Họ & Tên là bắt buộc"),
        StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), 
        UIHint("MemberTextBox")]
        public string FullName { get; set; }

        [Display(Name = "Số điện thoại"), Required(ErrorMessage = "Số điện thoại là bắt buộc"), StringLength(10, ErrorMessage = "Tối đa 10 ký tự")]
        [RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng."),
        UIHint("MemberTextBox")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ"), Required(ErrorMessage = "Địa chỉ là bắt buộc"), StringLength(200, ErrorMessage = "Địa chỉ không được dài quá 200 ký tự.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Tỉnh Thành là bắt buộc")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Quận Huyện là bắt buộc")]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "Xã Phường là bắt buộc")]
        public int WardId { get; set; }

        [Display(Name = "Địa chỉ mặc định")]
        public bool IsDefault { get; set; }

        public Member Member { get; set; }
        public City City { get; set; }
        public District District { get; set; }
        public Ward Ward { get; set; }
    }
}

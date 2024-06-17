using Evarosa.Models;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace Evarosa.ViewModels
{
    public class MemberVcmsViewModel
    {
        public IPagedList<Member> ListMember { get; set; }
        public Member Member { get; set; }

        public string? Term { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }

        [Display(Name = "Mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}

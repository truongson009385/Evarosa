﻿using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class Banner
    {
        public int Id { get; set; }
        [Display(Name = "Vị trí quảng cáo"), Required(ErrorMessage = "Bạn chưa chọn vị trí")]
        public int GroupId { get; set; }
        [Display(Name = "Tên banner"), Required(ErrorMessage = "Hãy nhập tên banner"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Slogan"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string? Slogan { get; set; }
        [Display(Name = "Hình ảnh"), StringLength(500)]
        public string? Image { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; } = true;
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string? Url { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên"), UIHint("NumberBox")]
        public int Sort { get; set; } = 1;
        [Display(Name = "Nội dung giới thiệu"), UIHint("EditorBox")]
        public string? Content { get; set; }
    }
}

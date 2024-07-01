﻿using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy nhập tên thành phố"), Display(Name = "Tên thành phố"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Name { get; set; }

        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int Sort { get; set; }

        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }

        [StringLength(20)]
        public string? Prefix { get; set; }

        public decimal ShipFee { get; set; }

        public virtual ICollection<District> Districts { get; set; }
    }
}
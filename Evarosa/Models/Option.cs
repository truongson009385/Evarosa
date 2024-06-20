using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class Option
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; } = 1;
        public OptionGroup Group { get; set; }
    }

    public enum OptionGroup
    {
        [Display(Name = "Màu sắc")]
        Color = 1,
        [Display(Name = "Dung tích")]
        Capacity,
        [Display(Name = "Combo")]
        Combo,
        [Display(Name = "Chất liệu")]
        Material,
        [Display(Name = "Loại")]
        Type
    }
}

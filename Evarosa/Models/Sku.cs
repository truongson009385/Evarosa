using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class Sku
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? SKU { get; set; }
        public int InStock { get; set; } = 0;
        public decimal Price { get; set; } = decimal.Zero;
        public decimal PriceSale { get; set; } = decimal.Zero;
        public Product Product { get; set; }
        public ICollection<OptionSku> OptionSkus { get; set; }
    }

    public class OptionSku
    {
        public int SkuId { get; set; }
        public int OptionId { get; set; }
        public string Value { get; set; }
        public virtual Sku Sku { get; set; }
        public Option Option { get; set; }
    }

    public class Option
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; } = 1;
        public ICollection<OptionSku> OptionSkus { get; set; }
    }
}

namespace Evarosa.Models
{
    public class Sku
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? SKU { get; set; }
        public int InStock { get; set; } = 0;
        public decimal? Price { get; set; } = 0;
        public Product Product { get; set; }
        public ICollection<OptionSku> OptionSkus { get; set; } = new List<OptionSku>();
    }

    public class OptionSku
    {
        public int SkuId { get; set; }
        public int OptionId { get; set; }
        public string Value { get; set; }
        public Sku Sku { get; set; }
        public Option Option { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Evarosa.Models
{
    public class Sku
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string? SKU { get; set; }

        public int InStock { get; set; } = 0;
        
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal Price { get; set; } = decimal.Zero;

        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal PriceSale { get; set; } = decimal.Zero;

        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal FinalPrice
        {
            get
            {
                return PriceSale != decimal.Zero ? PriceSale : Price;
            }
        }

        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal SavePrice
        {
            get
            {
                if (Price == 0 || PriceSale == 0 || PriceSale >= Price)
                {
                    return decimal.Zero;
                }
                return Price - PriceSale;
            }
        }

        public decimal Percent
        {
            get
            {
                if (Price == 0 || PriceSale == 0)
                {
                    return decimal.Zero;
                }

                decimal percent = (Price - PriceSale) / Price * 100;
                return Math.Round(percent, 0);
            }
        }

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

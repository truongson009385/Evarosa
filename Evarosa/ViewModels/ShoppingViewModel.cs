using Evarosa.Models;
using System.ComponentModel.DataAnnotations;

namespace Evarosa.ViewModels
{
    public class CartMiniViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal Total { get; set; } = 0;
        public IEnumerable<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

    public class ShoppingViewModel
    {
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal Total { get; set; } = 0;
        public IEnumerable<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

    public class CheckoutViewModel
    {
        public Order Order { get; set; } = new Order();
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal Total { get; set; } = 0;
        public IEnumerable<City> Cities { get; set; } = new List<City>();
        public IEnumerable<CartItem> CartItems { get; set; } = new List<CartItem>();

    }
}

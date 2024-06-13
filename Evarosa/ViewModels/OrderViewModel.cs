using Evarosa.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace Evarosa.ViewModels
{
    public class OrderViewModel
    {
        public IEnumerable<ListOrder> ListOrders { get; set; }

        public class ListOrder
        {
            public int MyProperty { get; set; }
        }
        public Order Order { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }

    public class ListOrderViewModel
    {
        public IPagedList<Order> Orders { get; set; }
        public IEnumerable<Order> Orderss { get; set; }
        public IEnumerable<PaymentType> PaymentTypes { get; set; } = new List<PaymentType>();

        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string MaDonhang { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string CustomerName { get; set; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string CustomerEmail { get; set; }
        [StringLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string CustomerMobile { get; set; }
        public OrderStatus? Status { get; set; }
        public PaymentType? Payment { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        [Required]
        public int PageSize { get; set; }
        public int? CityId { get; set; }
        public SelectList CitySelectList { get; set; }
    }

    public class PaymentTypeViewModel
    {
        public PaymentType PaymentType { get; set; }
        public IEnumerable<PaymentType> PaymentTypes { get; set; } = new List<PaymentType>();
        public IFormFile? Icon { get; set; } = null;
    }

    public class CityViewModel
    {
        public string Term { get; set; } = null;
        public IPagedList<City> ListCity { get; set; }
    }
}

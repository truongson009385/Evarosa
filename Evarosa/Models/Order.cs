using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Evarosa.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int? MemberId { get; set; }

        [Display(Name = "Mã đơn hàng")]
        [StringLength(50)]
        public string OrderCode { get; set; }

        [Display(Name = "Hình thức vận chuyển")]
        public TransportType TransportType { get; set; }

        [Display(Name = "Trạng thái đơn hàng")]
        public OrderStatus Status { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [Display(Name = "Ngày tạo"), DisplayFormat(DataFormatString = "{0:HH:mm - dd/MM/yyyy}")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Ngày vận chuyển"), DataType(DataType.DateTime)]
        public DateTime TransportDate { get; set; }

        [Display(Name = "Phí vận chuyển")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal ShipFee { get; set; }

        [Display(Name = "Thanh toán trước")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal Prepayment { get; set; }

        [Display(Name = "Hình thức thanh toán")]
		public PaymentType PaymentType { get; set; }

		[Display(Name = "Tỉnh, Thành phố"), Required(ErrorMessage = "Bạn hãy chọn Tỉnh/Thành")]
		public int CityId { get; set; }

		[Display(Name = "Quận, Huyện"), Required(ErrorMessage = "Bạn hãy chọn Quận/Huyện")]
		public int DistrictId { get; set; }

        [Display(Name = "Xã, Phường"), Required(ErrorMessage = "Bạn hãy chọn Xã/Phường")]
        public int WardId { get; set; }

        [Display(Name = "Địa chỉ"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Required(ErrorMessage = "Bạn hãy nhập địa chỉ")]
        public string Address { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal? Total { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal? TotalFee
        {
            get
            {
                return Total + ShipFee;
            }
        }

        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal? TotalDebt
        {
            get
            {
                return TotalFee - Prepayment;
            }
        }

        public Order()
        {
            OrderCode = "MĐH" + Id + DateTime.Now.ToString("mmddHH");
            CreateDate = DateTime.Now;
            TransportDate = DateTime.Now.AddDays(5);
            Status = OrderStatus.Pending;
            Prepayment = 0;
        }

        public virtual Customer Customer { get; set; }
		public virtual City City { get; set; }
		public virtual District District { get; set; }
        public virtual Ward Ward { get; set; }
        public Member? Member { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Họ & Tên"),
            StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), 
            Required(ErrorMessage = "Bạn hãy nhập họ và tên")]
        public string FullName { get; set; }

        [Display(Name = "Email"), RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Không đúng định dạng Email")
            , StringLength(100, ErrorMessage = "Tối đa 50 ký tự"), 
            Required(ErrorMessage = "Bạn hãy nhập địa chỉ email")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại"), RegularExpression(@"(\+84|0)(3[2-9]|5[689]|7[06789]|8[1-9]|9[0-9])(\d{7}|\d{8})", ErrorMessage = "Không đúng định dạng SĐT")
            , StringLength(20, ErrorMessage = "Tối đa 20 ký tự"), 
            Required(ErrorMessage = "Bạn hãy nhập số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Ghi chú"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string? Note { get; set; }
    }

    public enum PaymentType
    {
        [Display(Name = "Thanh toán khi nhận hàng")]
        PaymentOnDelivery = 1,
        [Display(Name = "Chuyển khoản")]
        Transfer = 2,
    }


    public enum TransportType
    {
        [Display(Name = "Giao tại cửa hàng")]
        InStoreDelivery = 1,
		[Display(Name = "Giao hàng tận nơi")]
		HomeDelivery = 2,
	}

    public enum OrderStatus
    {
        [Display(Name = "Chờ xác nhận")]
        Pending = 1,

        [Display(Name = "Đang xử lý")]
        Processing = 2,

        [Display(Name = "Đang vận chuyển")]
        Shipped = 3,

        [Display(Name = "Hoàn thành")]
        Completed = 4,

        [Display(Name = "Hủy đơn")]
        Canceled = 5,
    }
}
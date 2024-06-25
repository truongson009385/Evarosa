using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Evarosa.Data;
using Evarosa.Models;
using Evarosa.Services;
using Evarosa.ViewModels;
using X.PagedList;

namespace Evarosa.Controllers
{
    [Authorize(AuthenticationSchemes = "vcms", Roles = "Admin,Editor")]
    public class OrderController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public OrderController(UnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        #region City 

        public async Task<IActionResult> ListCityFee(int? page, string term = "")
        {
            int pageNumber = page ?? 1;

            var list = await _unitOfWork.City
                .GetPagedListAsync(
                    predicate: m => m.Name.Contains(term),
                    pageIndex: pageNumber,
                    pageSize: 10
                );

            var model = new CityViewModel
            {
                ListCity = list,
                Term = term,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCityFee(int id, decimal fee)
        {
            var city = await _unitOfWork.City.FindAsync(id);

            city.ShipFee = fee;
            _unitOfWork.Commit();
            return Json(new
            {
                success = true,
                name = city.Name,
            });
        }
        #endregion

        #region Order

        public ActionResult ListOrder(
            int? page,
            int? cityId,
            string madonhang,
            string fromdate,
            string todate,
            string customerName,
            string customerEmail,
            string customerMobile,
            OrderStatus? status,
            PaymentType? payment,
            int pageSize = 5
        )
        {
            var pageNumber = page ?? 1;
            var orders = _unitOfWork.Order
                .GetAll(
                    include: l => l.Include(m => m.District)
                    .Include(m => m.City)
                    .Include(m => m.Ward)
                    .Include(m => m.Customer)
                    .Include(m => m.OrderDetails),
                    orderBy: m => m.OrderByDescending(o => o.CreateDate)
                );

            if (!string.IsNullOrEmpty(madonhang))
            {
                orders = orders.Where(a => a.OrderCode.Contains(madonhang));
            }
            if (!string.IsNullOrEmpty(customerName))
            {
                orders = orders.Where(a => a.Customer.FullName.ToLower().Contains(customerName.ToLower()));
            }
            if (!string.IsNullOrEmpty(customerEmail))
            {
                orders = orders.Where(a => a.Customer.Email.ToLower().Contains(customerEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(customerMobile))
            {
                orders = orders.Where(a => a.Customer.PhoneNumber.Contains(customerMobile));
            }
            if (cityId.HasValue)
            {
                orders = orders.Where(a => a.CityId == cityId);
            }

            if (payment.HasValue)
            {
                orders = orders.Where(a => a.PaymentType == payment);
            }
            if (status.HasValue)
            {
                orders = orders.Where(a => a.Status == status);
            }

            var model = new ListOrderViewModel
            {
                Orders = orders.ToPagedList(pageNumber, pageSize),
                MaDonhang = madonhang,
                Status = status,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerMobile = customerMobile,
                FromDate = fromdate,
                ToDate = todate,
                PageSize = pageSize,
                Payment = payment,
                CityId = cityId,
                CitySelectList = new SelectList(_unitOfWork.City.GetAll(), "Id", "Name"),
            };

            return View(model);
        }

        public IActionResult LoadOrder(int orderId)
        {
            var order = _unitOfWork.Order
                .GetAll(
                    predicate: m => m.Id == orderId,
                    include: l => l.Include(m => m.Customer)
                    .Include(m => m.District)
                    .Include(m => m.City)
                    .Include(m => m.Ward)
                    .Include(m => m.OrderDetails)
                    .ThenInclude(m => m.Product)
                    .Include(m => m.OrderDetails)
                    .ThenInclude(m => m.Sku)
                ).FirstOrDefault();

            if (order == null) return NotFound();

            var model = new OrderViewModel
            {
                Order = order
            };

            return PartialView(model);
        }

        [HttpPost]
        public async Task<bool> UpdateOrder(OrderStatus? status, int orderId = 0)
        {
            var order = await _unitOfWork.Order.FindAsync(orderId);

            if (order == null)
            {
                return false;
            }

            if (status.HasValue)
            {
                order.Status = status.Value;
            }

            //if (payment.HasValue)
            //{
            //    order.TypePay = payment.Value;
            //}

            _unitOfWork.Order.Update(order);
            _unitOfWork.Commit();
            return true;
        }

        [HttpPost]
        public async Task<bool> UpdateOrderNotice(string? notice, PaymentType payment, OrderStatus status, decimal thanhtoantruoc = 0, decimal ship = 0, int orderId = 0)
        {
            var order = await _unitOfWork.Order.GetAll(
                    predicate: m => m.Id == orderId,
                    include: m => m.Include(l => l.Customer),
                    disableTracking: false
                ).FirstOrDefaultAsync();

            if (order == null)
            {
                return false;
            }

            order.PaymentType = payment;
            order.Status = status;
            order.ShipFee = ship;
            order.Prepayment = thanhtoantruoc;
            if (!string.IsNullOrEmpty(notice))
                order.Customer.Note = notice;

            _unitOfWork.Order.Update(order);
            _unitOfWork.Commit();
            return true;
        }

        [HttpPost]
        public async Task<bool> DeleteOrder(int orderId = 0)
        {

            var order = await _unitOfWork.Order.FindAsync(orderId);

            if (order == null)
            {
                return false;
            }

            order.Status = OrderStatus.Canceled;
            _unitOfWork.Order.Update(order);
            _unitOfWork.Commit();
            return true;
        }

        [HttpPost]
        public async Task<bool> ParmanentDeleteOrder(int orderId = 0)
        {
            var order = await _unitOfWork.Order.FindAsync(orderId);

            if (order == null || order.Status != OrderStatus.Canceled)
            {
                return false;
            }


            _unitOfWork.Order.Delete(order);
            _unitOfWork.Commit();
            return true;

        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Rollback();
            base.Dispose(disposing);
        }
    }
}

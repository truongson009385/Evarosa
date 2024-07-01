using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Evarosa.Data;
using Evarosa.Models;
using Evarosa.Services.Impl;
using Evarosa.Services;
using Evarosa.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Evarosa.Utils;

namespace Evarosa.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly IAppService _appService;
        private readonly IWebHostEnvironment _env;

        public ShoppingCartController(
            UnitOfWork unitOfWork, 
            IMailService mailService,
            IAppService appService,
            IWebHostEnvironment env
        )
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
            _appService = appService;
            _env = env;
        }

        public ShoppingService cart => ShoppingService.GetCart(HttpContext, _unitOfWork);

        public IActionResult CartMini()
        {
            var model = new CartMiniViewModel
            {
                CartItems = cart.GetCartItems(),
                Total = cart.GetTotal(),
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity, int skuId)
        {
            try
            {
                var product = await _unitOfWork.Product.FindAsync(id);
                var sku = await _unitOfWork.Sku.FindAsync(skuId);

                cart.AddToCart(product, sku, quantity);

                return Json(new
                {
                    status = true,
                    msg = "Thêm vào giỏ hàng thành công!",
                    count = cart.GetCount(),
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    msg = ex.Message,
                    count = cart.GetCount(),
                });
            }
        }

        [HttpPost]
        public IActionResult UpdateFromCart(string recordId, int quantity)
        {
            try
            {
                cart.UpdateFromCart(recordId, quantity);

                return Json(new
                {
                    status = true,
                    msg = "Thêm vào giỏ hàng thành công!",
                    count = cart.GetCount(),
                    itemTotal = cart.GetItemTotal(recordId).ToString("N0") + " VNĐ",
                    total = cart.GetTotal().ToString("N0") + " VNĐ",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    msg = ex.Message,
                    count = cart.GetCount(),
                });
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(string recordId)
        {
            try
            {
                cart.RemoveFromCart(recordId);

                return Json(new
                {
                    status = true,
                    msg = "Đã xóa sản phẩm khỏi giỏ hàng!",
                    count = cart.GetCount(),
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    msg = ex.Message,
                    count = cart.GetCount(),
                });
            }
        }

        [Route("gio-hang")]
        public IActionResult Index()
        {
            var model = new ShoppingViewModel
            {
                CartItems = cart.GetCartItems(),
                Total = cart.GetTotal(),
            };
            return View(model);
        }

        [Route("thanh-toan")]
        public async Task<IActionResult> Checkout()
        {
            if (cart.GetCount() <= 0)
                return RedirectToAction("Index");

            var cities = await _unitOfWork.City.GetAllAsync();

            var model = new CheckoutViewModel
            {
                Cities = cities,
                CartItems = cart.GetCartItems(),
                Total = cart.GetTotal(),
                Order = new Order()
            };

            if (User.Identity.IsAuthenticated)
            {
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var member = _unitOfWork.Member.GetAll(
                        predicate: m => m.Email == emailClaim,
                        include: m => m.Include(l => l.MemberAddresses.OrderByDescending(o => o.IsDefault))
                    ).FirstOrDefault();

                if (member != null)
                {
                    var address = member.MemberAddresses.FirstOrDefault();

                    model.Order.MemberId = member.Id;
                    model.Order.Customer = new Customer
                    {
                        FullName = member.FullName,
                        Email = member.Email,
                        PhoneNumber = member.PhoneNumber
                    };

                    if (address != null)
                    {
                        model.Order.CityId = address.CityId;
                        model.Order.DistrictId = address.DistrictId;
                        model.Order.WardId = address.WardId;

                        ViewBag.Districts = new SelectList(await _unitOfWork.District.GetAllAsync(predicate: d => d.CityId == address.CityId), "Id", "Name");
                        ViewBag.Wards = new SelectList(await _unitOfWork.Ward.GetAllAsync(predicate: w => w.DistrictID == address.DistrictId), "ID", "Name");

                        model.Order.Address = address.Address;
                        model.Order.Customer.PhoneNumber = address.PhoneNumber;
                        model.Order.Customer.FullName = address.FullName;
                    }

                }
            }

            return View(model);
        }

        [HttpPost("thanh-toan")]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            try
            {
                model.Order.ShipFee = _unitOfWork.City.Find(model.Order.CityId).ShipFee;
                model.Order.Total = model.Order.ShipFee + cart.GetTotal();
                await _unitOfWork.Order.InsertAsync(model.Order);
                await _unitOfWork.CommitAsync();

                cart.CreateOrder(model.Order);

                var order = await _unitOfWork.Order.GetAll(
                        predicate: m => m.OrderCode == model.Order.OrderCode,
                        include: l => l.Include(m => m.OrderDetails)
                            .ThenInclude(m => m.Product)
                            .Include(m => m.Customer)
                            .Include(m => m.District)
                            .Include(m => m.City)
                            .Include(m => m.Ward)
                    ).FirstOrDefaultAsync();

                if (order == null) return NotFound();

                #region Customer
                string pathInvoiceEmail = Path.Combine(_env.ContentRootPath, "EmailTemplates\\InvoiceEmail.html");
                using (StreamReader reader = new StreamReader(pathInvoiceEmail))
                {
                    string subject = "Chúng tôi đã nhận được đơn hàng: " + order.OrderCode;
                    string body = string.Empty;

                    body = reader.ReadToEnd();

                    string logoUrl = $"{Request.Scheme}://{Request.Host}/contents/system/{_appService.Config.Image}";

                    body = body.Replace("{OrderCode}", order.OrderCode);
                    body = body.Replace("{PaymentType}", order.PaymentType.GetDisplayName());
                    body = body.Replace("{CreateDate}", order.CreateDate.ToString("HH:mm - dd/MM/yyyy"));
                    body = body.Replace("{Total}", order.Total?.ToString("N0") + " VNĐ");
                    body = body.Replace("{TotalFee}", order.TotalFee?.ToString("N0") + " VNĐ");
                    body = body.Replace("{ShipFee}", order.ShipFee.ToString("N0") + " VNĐ");
                    body = body.Replace("{FullName}", order.Customer.FullName);
                    body = body.Replace("{Email}", order.Customer.Email);
                    body = body.Replace("{PhoneNumber}", order.Customer.PhoneNumber);
                    body = body.Replace("{Note}", order.Customer.Note);
                    body = body.Replace("{Address}", $"{order.Address}, {order.Ward.Name}, {order.District.Name}, {order.City.Name}");

                    body = body.Replace("{Hotline}", _appService.Config.Hotline);
                    body = body.Replace("{SystemEmail}", _appService.Config.Email);

                    string itemsHtml = "";

                    foreach (var item in order.OrderDetails)
                    {
                        string itemSku = item.Sku == null ? "" : $" <i>SKU: {item.Sku.SKU}</i>";
                        string itemHtml = $@"
                        <p style=""font-size:14px; margin:0; padding:10px; border:solid 1px #ddd; font-weight:bold;"">
                            <span style=""display:block; font-size:13px; font-weight:normal;"">{item.Product.Name} {itemSku}</span>
                            {item.Amount.ToString("N0")} VNĐ
                            <b style=""font-size:12px; font-weight:300;"">/ x{item.Quantity}/ {item.UnitPrice.ToString("N0")} VNĐ</b>
                        </p>";

                        itemsHtml += itemHtml;
                    }

                    body = body.Replace("{Items}", itemsHtml);

                    var mailData = new MailData
                    {
                        EmailToId = model.Order.Customer.Email,
                        EmailSubject = subject,
                        EmailBody = body
                    };

                    await _mailService.SendEmailAsync(mailData);
                }
                #endregion

                #region Admin
                if (!string.IsNullOrEmpty(_appService.Config.Email))
                {
                    string pathAlertOrder = Path.Combine(_env.ContentRootPath, "EmailTemplates\\AlertOrder.html");
                    using (StreamReader reader = new StreamReader(pathAlertOrder))
                    {
                        string subject = "Bạn có đơn hàng mới từ: " + Request.Host + " | " + order.OrderCode;
                        string body = string.Empty;

                        body = reader.ReadToEnd();

                        string? urlOrderVcms = Url.Action("ListOrder", "Order", new { madonhang = order.OrderCode }, protocol: Request.Scheme);

                        body = body.Replace("{OrderCode}", order.OrderCode);
                        body = body.Replace("{CreateDate}", order.CreateDate.ToString("HH:mm - dd/MM/yyyy"));
                        body = body.Replace("{Company}", _appService.Config.Title);
                        body = body.Replace("{Url}", urlOrderVcms);

                        var mailData = new MailData
                        {
                            EmailToId = _appService.Config.Email,
                            EmailSubject = subject,
                            EmailBody = body
                        };

                        await _mailService.SendEmailAsync(mailData);
                    }
                }
                #endregion

                return RedirectToAction("CheckoutComplete", new { MaDonHang = model.Order.OrderCode });
            }
            catch (Exception ex)
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
            }
        }

        [Route("/thanh-toan-thanh-cong")]
        public async Task<IActionResult> CheckoutComplete(string MaDonHang)
        {
            var order = await _unitOfWork.Order
                    .GetAll(
                        predicate: m => m.OrderCode.Contains(MaDonHang),
                        include: l => l.Include(m => m.Customer)
                    ).FirstOrDefaultAsync();

            if (order == null) return NotFound();

            var model = new CheckoutViewModel
            {
                Order = order,
            };
            return View(model);
        }

        [Route("tinh-thanh")]
        public IActionResult GetCities(string? searchTerm)
        {
            var data = _unitOfWork.City.GetAll();

            if (searchTerm != null)
            {
                data = data.Where(m => m.Name.Contains(searchTerm));
            }

            return Json(new
            {
                cities = data.ToList()
            });
        }

        [Route("quan-huyen")]
        public IActionResult GetDistricts(int cityId, string? searchTerm)
        {
            var data = _unitOfWork.District.GetAll();

            data = data.Where(d => d.CityId == cityId);

            if (searchTerm != null)
            {
                data = data.Where(m => m.Name.Contains(searchTerm));
            }

            return Json(new
            {
                districts = data.ToList()
            });
        }

        [Route("xa-phuong")]
        public IActionResult GetWards(int districtId, string? searchTerm)
        {
            var data = _unitOfWork.Ward.GetAll();

            data = data.Where(d => d.DistrictID == districtId);

            if (searchTerm != null)
            {
                data = data.Where(m => m.Name.Contains(searchTerm));
            }

            return Json(new
            {
                wards = data.ToList()
            });
        }

        [Route("shipfee")]
        public async Task<IActionResult> GetShipFee(int cityId)
        {
            var data = await _unitOfWork.City.FindAsync(cityId);

            if (data != null)
            {
                return Json(new
                {
                    status = true,
                    fee = data.ShipFee.ToString("N0") + " VNĐ",
                    total = (data.ShipFee + cart.GetTotal()).ToString("N0") + " VNĐ",
                });
            }

            return Json(new
            {
                status = false
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Evarosa.Data;
using Evarosa.Models;
using Evarosa.Services.Impl;
using Evarosa.Services;
using Evarosa.ViewModels;
using System.Diagnostics;

namespace Evarosa.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly IAppService _appService;

        public ShoppingCartController(UnitOfWork unitOfWork, IMailService mailService, IAppService appService)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
            _appService = appService;
        }

        public ShoppingService cart
        {
            get
            {
                return ShoppingService.GetCart(HttpContext, _unitOfWork);
            }
        }
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
        public async Task<IActionResult> AddToCart(int id, int quantity)
        {
            try
            {
                var product = await _unitOfWork.Product.FindAsync(id);

                cart.AddToCart(product, quantity);

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
            if (cart.GetCount() <= 0) return RedirectToAction("Index");

            var cities = await _unitOfWork.City.GetAllAsync();

            var model = new CheckoutViewModel
            {
                Cities = cities,
                CartItems = cart.GetCartItems(),
                Total = cart.GetTotal(),
            };
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

                var subject = "Chúng tôi đã nhận được đơn hàng: " + model.Order.OrderCode;
                var body = $"<p>Người mua hàng: {model.Order.Customer.FullName},</p>" +
                            $"<p>Email: {model.Order.Customer.Email},</p>" +
                            $"<p>Điện thoại: {model.Order.Customer.PhoneNumber},</p>" +
                            $"<p>Cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi.</p>";

                var mailData = new MailData
                {
                    EmailToId = model.Order.Customer.Email,
                    EmailSubject = subject,
                    EmailBody = body
                };

                await _mailService.SendEmailAsync(mailData);

                return RedirectToAction("CheckoutComplete", new { MaDonHang = model.Order.OrderCode });
            }
            catch
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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

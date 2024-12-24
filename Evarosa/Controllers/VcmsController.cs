using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Evarosa.Data;
using Evarosa.Models;
using Evarosa.Services;
using Evarosa.Utils;
using Evarosa.ViewModels;

namespace Evarosa.Controllers
{
    [Authorize(AuthenticationSchemes = "vcms")]
    public class VcmsController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IAppService _appService;
        private readonly string _pepper;
        private readonly int _iteration = 3;

        public VcmsController(UnitOfWork unitOfWork, IFileService fileService, IAppService appService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _appService = appService;
            _pepper = Environment.GetEnvironmentVariable("vico@123");
        }

        #region Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(AdminForm model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var admin = await _unitOfWork.Admin.GetAll(predicate: a => a.Username == model.Username).FirstOrDefaultAsync();

            if (admin == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại");
                return View(model);
            }

            var passwordHash = HtmlHelpers.ComputeHash(model.Password, _pepper, _iteration);
            if (admin.Password != passwordHash)
            {
                ModelState.AddModelError("", "Mật khẩu không đúng");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, admin.Role.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "vcms");
            await HttpContext.SignInAsync("vcms", new ClaimsPrincipal(claimsIdentity));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Vcms");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("vcms");
            return RedirectToAction(nameof(Login));
        }
        #endregion

        #region Admin

        public IActionResult Index()
        {
            var model = new InfoAdminViewModel
            {
                Admins = _unitOfWork.Admin.Count(),
                Articles = _unitOfWork.Article.Count(),
                Banners = _unitOfWork.Banner.Count(),
                Members = _unitOfWork.Member.Count(),
                Orders = _unitOfWork.Order.Count(),
                Contacts = _unitOfWork.Contact.Count(),
                Products = _unitOfWork.Product.Count(),
            };
            return View(model);
        }

        public IActionResult CkFinder()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin(string result = "")
        {
            ViewBag.Result = result;

            var listAdmin = await _unitOfWork.Admin.GetAllAsync();
            var model = new AdminViewModel
            {
                Admins = listAdmin
            };
            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin(AdminViewModel model)
        {
            var admin = await _unitOfWork.Admin.GetAll(predicate: m => m.Username == model.Admin.Username).FirstOrDefaultAsync();

            if (admin != null)
            {
                ViewBag.ErrorMessage = $"Tên đăng nhập {model.Admin.Username} đã tồn tại";
                return View();
            }
            else
            {
                model.Admin.Password = HtmlHelpers.ComputeHash(model.Admin.Password, _pepper, _iteration);

                _unitOfWork.Admin.Insert(model.Admin);
                _unitOfWork.Commit();
            }
            return RedirectToAction("CreateAdmin", new { result = "success" });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAdmin(int adminId = 0)
        {
            var admin = await _unitOfWork.Admin.FindAsync(adminId);
            if (admin == null)
            {
                return RedirectToAction("CreateAdmin");
            }

            admin.Password = null;
            var listAdmin = await _unitOfWork.Admin.GetAllAsync();

            var model = new AdminViewModel
            {
                Admin = admin,
                Admins = listAdmin
            };

            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAdmin(AdminViewModel model)
        {
            var admin = await _unitOfWork.Admin.FindAsync(model.Admin.Id);
            byte[] salt;

            if (admin == null)
            {
                return RedirectToAction("CreateAdmin");
            }
            if (admin.Username != model.Admin.Username)
            {
                var exists = await _unitOfWork.Admin.GetAll(predicate: a => a.Username == model.Admin.Username).FirstOrDefaultAsync();
                if (exists != null)
                {
                    ModelState.AddModelError("", @"Tên đăng nhập này có rồi");
                    return View(model);
                }
                admin.Username = model.Admin.Username;
            }

            admin.Active = model.Admin.Active;
            admin.Role = model.Admin.Role;

            if (model.Admin.Password != null)
            {
                admin.Password = HtmlHelpers.ComputeHash(model.Admin.Password, _pepper, _iteration);
            }

            _unitOfWork.Admin.Update(admin);
            _unitOfWork.Commit();
            return RedirectToAction("CreateAdmin", new { result = "update" });
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<bool> DeleteAdmin(string username)
        {
            var admin = await _unitOfWork.Admin.GetAll(predicate: a => a.Username == username).FirstOrDefaultAsync();
            if (admin == null)
            {
                return false;
            }
            if (username == "admin")
            {
                return false;
            }
            _unitOfWork.Admin.Delete(admin);
            _unitOfWork.Commit();
            return true;
        }

        public IActionResult ChangePassword(int result = 0)
        {
            ViewBag.Result = result;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var admin = await _unitOfWork.Admin.GetAll(predicate: a => a.Username == User.Identity.Name).FirstOrDefaultAsync();

            if (admin == null) return NotFound();

            var passwordHash = HtmlHelpers.ComputeHash(model.OldPassword, _pepper, _iteration);

            if (admin.Password == passwordHash)
            {
                admin.Password = HtmlHelpers.ComputeHash(model.Password, _pepper, _iteration);

                _unitOfWork.Admin.Update(admin);
                _unitOfWork.Commit();
                return RedirectToAction("ChangePassword", new { result = 1 });
            }
            return View(model);
        }
        #endregion

        #region ConfigSite
        [Authorize(Roles = "Admin")]
        public IActionResult ConfigSite(string result = "")
        {
            ViewBag.Result = result;
            var config = _unitOfWork.ConfigSite.GetAll().FirstOrDefault() ?? new ConfigSite();
            return View(config);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfigSite(ConfigSite model)
        {
            var config = await _unitOfWork.ConfigSite.GetAll(disableTracking: false).FirstOrDefaultAsync();
            var Favicon = Request.Form.Files["Favicon"];
            var Image = Request.Form.Files["Image"];
            var AboutImage = Request.Form.Files["AboutImage"];

            if (config == null)
            {
                _unitOfWork.ConfigSite.Insert(model);
                await _unitOfWork.CommitAsync();
            }
            else
            {
                if (Favicon is { Length: > 0 })
                {
                    var img = await _fileService.UploadFileAsync("system", Favicon);

                    config.Favicon = img.FileName;
                }
                if (Image is { Length: > 0 })
                {
                    var img = await _fileService.UploadFileAsync("system", Image);

                    config.Image = img.FileName;
                }
                if (AboutImage is { Length: > 0 })
                {
                    var img = await _fileService.UploadFileAsync("system", AboutImage);

                    config.AboutImage = img.FileName;
                }

                //Cấu hình
                config.Title = model.Title;
                config.Description = model.Description;
                config.GoogleAnalytics = model.GoogleAnalytics;

                //Mạng xã hội
                config.Facebook = model.Facebook;
                config.GoogleMapLink = model.GoogleMapLink;
                config.WhatsApp = model.WhatsApp;
                config.Viber = model.Viber;
                config.Twitter = model.Twitter;
                config.Instagram = model.Instagram;
                config.Linkedin = model.Linkedin;
                config.Zalo = model.Zalo;
                config.Youtube = model.Youtube;

                //Địa chỉ
                config.Place = model.Place;
                config.GoogleMap = model.GoogleMap;

                //Liên hệ
                config.Email = model.Email;
                config.Hotline = model.Hotline;
                config.LiveChat = model.LiveChat;
                config.ContactInfo = model.ContactInfo;

                //Giới thiệu
                config.AboutTitle = model.AboutTitle;
                config.AboutText = model.AboutText;
                config.AboutUrl = model.AboutUrl;
                config.FooterInfo = model.FooterInfo;

                await _unitOfWork.CommitAsync();

                _appService.ReloadConfig();

                return RedirectToAction("ConfigSite", "Vcms", new { result = "success" });
            }
            return View("ConfigSite", model);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Rollback();
            base.Dispose(disposing);
        }
    }
}

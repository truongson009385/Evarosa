using Evarosa.Data;
using Evarosa.Models;
using Evarosa.Services;
using Evarosa.Utils;
using Evarosa.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Evarosa.Controllers
{
    [Authorize(AuthenticationSchemes = "member")]
    public class MemberController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IAppService _appService;
        private readonly ILogger<MemberController> _logger;
        private readonly string _pepper;
        private readonly int _iteration = 3;

        public MemberController(UnitOfWork unitOfWork, IFileService fileService, IAppService appService, ILogger<MemberController> logger)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _appService = appService;
            _logger = logger;
            _pepper = Environment.GetEnvironmentVariable("vico@123");
        }

        private Member? GetMember()
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var member = _unitOfWork.Member.GetAll(
                    predicate: m => m.Email == emailClaim
                ).FirstOrDefault();
            return member;
        }

        [HttpGet("member")]
        public IActionResult Index()
        {
            var member = GetMember();

            if (member == null) return NotFound();

            var model = new MemberViewModel
            {
                Member = member
            };
            return View(model);
        }

        [HttpPost("dang-xuat")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("member");
            return RedirectToAction(nameof(Login));
        }

        #region AllowAnonymous
        [AllowAnonymous]
        [HttpGet("dang-ky")]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("dang-ky")]
        public async Task<IActionResult> Register(MemberForm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var exists = await _unitOfWork.Member.GetAll(predicate: m => m.Email == model.Email).FirstOrDefaultAsync();

                if (exists != null)
                {
                    ModelState.AddModelError("", "Địa chỉ email đã được sử dụng.");
                    return View(model);
                }

                var member = new Member
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Password = HtmlHelpers.ComputeHash(model.Password, _pepper, _iteration),
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    BirthDate = model.BirthDate
                };

                await _unitOfWork.Member.InsertAsync(member);
                _unitOfWork.Commit();
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi trong quá trình đăng ký.");
                ModelState.AddModelError("", "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.");
                return View(model);
            }
        }

        [AllowAnonymous]
        [HttpGet("dang-nhap")]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("dang-nhap")]
        public async Task<IActionResult> Login(MemberLogin model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var member = await _unitOfWork.Member.GetAll(predicate: a => a.Email == model.Email).FirstOrDefaultAsync();

                if (member == null)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại");
                    return View(model);
                }

                var passwordHash = HtmlHelpers.ComputeHash(model.Password, _pepper, _iteration);
                if (member.Password != passwordHash)
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, member.Id.ToString()),
                    new Claim(ClaimTypes.Name, member.FullName),
                    new Claim(ClaimTypes.Email, member.Email),
                    new Claim(ClaimTypes.MobilePhone, member.PhoneNumber),
                    new Claim(ClaimTypes.StreetAddress, member.Address),
                    new Claim(ClaimTypes.DateOfBirth, member.BirthDate.ToString()),
                };

                var claimsIdentity = new ClaimsIdentity(claims, "member");
                await HttpContext.SignInAsync("member", new ClaimsPrincipal(claimsIdentity));

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Member");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi trong quá trình đăng nhập.");
                ModelState.AddModelError("", "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.");
                return View(model);
            }
        }
        #endregion
    }
}

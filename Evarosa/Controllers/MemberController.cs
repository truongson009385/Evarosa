using Evarosa.Data;
using Evarosa.Models;
using Evarosa.Services;
using Evarosa.Services.Impl;
using Evarosa.Utils;
using Evarosa.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Evarosa.Controllers
{
    [Authorize(AuthenticationSchemes = "member")]
    public class MemberController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IAppService _appService;
        private readonly IMailService _mailService;
        private readonly ILogger<MemberController> _logger;
        private readonly string _pepper;
        private readonly int _iteration = 3;

        public MemberController(
            UnitOfWork unitOfWork, 
            IFileService fileService, 
            IAppService appService, 
            ILogger<MemberController> logger, 
            IMailService mailService
        )
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _appService = appService;
            _logger = logger;
            _pepper = Environment.GetEnvironmentVariable("vico@123");
            _mailService = mailService;
        }

        public ShoppingService cart => ShoppingService.GetCart(HttpContext, _unitOfWork);

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
            ViewBag.Message = TempData["Message"];

            var member = GetMember();

            if (member == null) return NotFound();

            var model = new MemberViewModel
            {
                Member = member
            };
            return View(model);
        }

        [HttpPost("member")]
        public async Task<IActionResult> Index(MemberViewModel model)
        {
            var member = GetMember();

            if (member == null) return NotFound();

            var exists = _unitOfWork.Member.GetAll(
                predicate: m => m.Email == model.Member.Email && m.Id != member.Id
            ).Any();

            if (exists)
            {
                ModelState.AddModelError("", "Địa chỉ email đã được sử dụng.");
                return View(model);
            }

            member.Email = model.Member.Email;
            member.FullName = model.Member.FullName;
            member.Address = model.Member.Address;
            member.PhoneNumber = model.Member.PhoneNumber;
            member.BirthDate = model.Member.BirthDate;

            _unitOfWork.Member.Update(member);
            _unitOfWork.Commit();


            var claims = User.Claims.ToList();
            var claim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (claim != null)
            {
                claims.Remove(claim);
                claims.Add(new Claim(ClaimTypes.Email, member.Email));

                var appIdentity = new ClaimsIdentity(claims, "member");
                var user = new ClaimsPrincipal(appIdentity);

                await HttpContext.SignInAsync("member", user);
            }

            TempData["Message"] = "success|Cập nhật thông tin tài khoản thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("dang-xuat")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("member");

            return RedirectToAction(nameof(Login));
        }

        [HttpGet("member/doi-mat-khau")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("member/doi-mat-khau")]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin.");
                return View(model);
            }

            var member = GetMember();

            if (member == null) return NotFound();

            if (HtmlHelpers.ComputeHash(model.OldPassword, _pepper, _iteration) != member.Password)
            {
                ModelState.AddModelError("", "Mật khẩu không đúng.");
                return View(model);
            }

            member.Password = HtmlHelpers.ComputeHash(model.Password, _pepper, _iteration);

            _unitOfWork.Member.Update(member);
            _unitOfWork.Commit();
            return RedirectToAction(nameof(ChangePassword));
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
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin.");
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
                    BirthDate = model.BirthDate,
                    EmailConfirmation = await GenerateToken(model.Email)
                };

                await _unitOfWork.Member.InsertAsync(member);
                _unitOfWork.Commit();

                var callbackurl = Url.Action("EmailConfirmation", "Member", new { userId= member.Id, code = member.EmailConfirmation }, protocol: HttpContext.Request.Scheme);

                var mailData = new MailData
                {
                    EmailToId = model.Email,
                    EmailToName = member.FullName,
                    EmailSubject = "Email xác nhận",
                    EmailBody = "Hãy bấm vào " + "<a href=\"" + callbackurl + "\">đường dẫn</a>" + " để xác minh tài khoản."
                };
                await _mailService.SendEmailAsync(mailData);
                return RedirectToAction(nameof(EmailConfirmation));
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

                if (member.EmailConfirmation != null)
                {
                    ModelState.AddModelError("", "Tài khoản chưa được xác minh. Vui lòng kiểm tra email của bạn.");
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

                cart.MigrateCart(member.Email);

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

        [AllowAnonymous]
        [HttpGet("quen-mat-khau")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("quen-mat-khau")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var member = await _unitOfWork.Member.GetAll(predicate: m => m.Email == model.Email).FirstOrDefaultAsync();
                if (member == null)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại");
                    return View(model);
                }
                member.Token = await GenerateToken(model.Email);
                var callbackurl = Url.Action("ResetPassword", "Member", new { userId = member.Id, code = member.Token }, protocol: HttpContext.Request.Scheme);

                _unitOfWork.Member.Update(member);
                await _unitOfWork.CommitAsync();

                var mailData = new MailData
                {
                    EmailToId = model.Email,
                    EmailToName = member.FullName,
                    EmailSubject = "Reset Email Confirmation",
                    EmailBody = "Please reset password by going to this " + "<a href=\"" + callbackurl + "\">link</a>"
                };
                await _mailService.SendEmailAsync(mailData);

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet("forgot-password-confirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View("EmailConfirmation");
        }

        [AllowAnonymous]
        [HttpGet("reset-password/{code}")]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? NotFound() : View();
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordStore(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin.");
                return View(model);
            }

            var member = await _unitOfWork.Member.GetAll(predicate: m => m.Email == model.Email && m.Token == model.Code).FirstOrDefaultAsync();
            if (member == null)
            {
                ModelState.AddModelError("", "Invalid token.");
                return View();
            }

            member.Password = HtmlHelpers.ComputeHash(model.Password, _pepper, _iteration);
            member.Token = null;

            _unitOfWork.Member.Update(member);
            await _unitOfWork.CommitAsync();

            TempData["Message"] = "success|Đặt lại mật khẩu thành công.";
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Error", "Home");
        }

        [AllowAnonymous]
        [HttpGet("xac-minh-tai-khoan/{userId}/{code}")]
        public async Task<IActionResult> EmailConfirmation(int userId, string code)
        {
            var member = await _unitOfWork.Member.GetAll(predicate: m => m.Id == userId && m.EmailConfirmation == code).FirstOrDefaultAsync();

            if (member == null)return NotFound();

            member.EmailConfirmation = null;

            _unitOfWork.Member.Update(member);
            await _unitOfWork.CommitAsync();

            return RedirectToAction(nameof(Login));
        }
        #endregion

        private async Task<string> GenerateToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("vicogroup_secret_token_key_123456789");

            var expirationTime = DateTime.UtcNow.AddDays(30);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Expiration, expirationTime.ToString("o"))
                }),
                Expires = expirationTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public ClaimsPrincipal GetClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("vicogroup_member_token_123");

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims));
        }

        [HttpPost("member/upload-avatar")]
        public async Task<IActionResult> UploadAvatar()
        {
            var avatar = Request.Form.Files["avatar"];

            if (avatar != null && avatar.Length > 0)
            {
                var member = GetMember();

                if (member == null) return NotFound();

                var file = await _fileService.UploadFileAsync("members", avatar);
                member.Image = file.FileName;

                _unitOfWork.Member.Update(member);
                await _unitOfWork.CommitAsync();
            }

            return Json(new { success = true });
        }

        [Route("member/dia-chi")]
        public async Task<IActionResult> ListAddress(int? page)
        {
            var list = await _unitOfWork.MemberAddress.GetPagedListAsync(
                    include: m => m
                        .Include(l => l.City)
                        .Include(l => l.District)
                        .Include(l => l.Ward),
                    pageIndex: page ?? 1,
                    pageSize: 10
                );

            var model = new MemberAddressViewModel
            {
                ListMemberAddress = list
            };

            return View(model);
        }

        public async Task<IActionResult> AddAddress()
        {
            ViewBag.Cities = new SelectList(await _unitOfWork.City.GetAllAsync(), "Id", "Name");

            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(MemberAddress model)
        {  
            try
            {
                model.MemberId = GetMember().Id;
                AddOrUpdateAddress(model);

                TempData["Message"] = "success|Thêm mới thông tin địa chỉ thành công.";
                return RedirectToAction(nameof(ListAddress));
            }
            catch (Exception ex)
            {
                TempData["Message"] = "danger|Quá trình không xảy ra lỗi.";
                return RedirectToAction(nameof(ListAddress));
            }
        }

        public async Task<IActionResult> UpdateAddress(int Id)
        {
            var address = await _unitOfWork.MemberAddress.FindAsync(Id);

            if (address == null) return NotFound();

            ViewBag.Cities = new SelectList(await _unitOfWork.City.GetAllAsync(), "Id", "Name");
            ViewBag.Districts = new SelectList(await _unitOfWork.District.GetAllAsync(predicate: d => d.CityId == address.CityId), "Id", "Name");
            ViewBag.Wards = new SelectList(await _unitOfWork.Ward.GetAllAsync(predicate: w => w.DistrictID == address.DistrictId), "ID", "Name");

            return PartialView(address);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress(MemberAddress model)
        {
            try
            {
                AddOrUpdateAddress(model);

                TempData["Message"] = "success|Cập nhật thông tin địa chỉ thành công.";
                return RedirectToAction(nameof(ListAddress));
            }
            catch (Exception ex)
            {
                TempData["Message"] = "danger|Quá trình không xảy ra lỗi.";
                return RedirectToAction(nameof(ListAddress));
            }
        }

        public async Task<IActionResult> DeleteAddress(int Id)
        {
            var address = await _unitOfWork.MemberAddress.FindAsync(Id);

            if (address == null) return NotFound();

            _unitOfWork.MemberAddress.Delete(address);
            await _unitOfWork.CommitAsync();

            TempData["Message"] = "success|Xóa thông tin địa chỉ thành công.";
            return RedirectToAction(nameof(ListAddress));
        }

        public void AddOrUpdateAddress(MemberAddress address)
        {
            if (address.IsDefault)
            {
                var otherAddresses = _unitOfWork.MemberAddress
                    .GetAll(predicate: a => a.MemberId == address.MemberId && a.Id != address.Id, disableTracking: false);

                foreach (var otherAddress in otherAddresses)
                {
                    otherAddress.IsDefault = false;
                }

                _unitOfWork.MemberAddress.Update(otherAddresses);
            }

            if (address.Id == 0)
            {
                _unitOfWork.MemberAddress.Insert(address);
            }
            else
            {
                _unitOfWork.MemberAddress.Update(address);
            }

            _unitOfWork.Commit();
        }

        public async Task<IActionResult> ListOrder(int? page)
        {
            var list = await _unitOfWork.Order.GetPagedListAsync(
                    predicate: o => o.MemberId == GetMember().Id,
                    include: m => m
                        .Include(l => l.City)
                        .Include(l => l.District)
                        .Include(l => l.Ward),
                    pageIndex: page ?? 1,
                    pageSize: 10
                );

            var model = new MemberOrderViewModel
            {
                ListOrder = list
            };
            return View(model);
        }
    }
}

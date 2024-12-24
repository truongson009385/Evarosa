using Evarosa.Data;
using Evarosa.Utils;
using Evarosa.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Evarosa.Controllers
{
    [Authorize(AuthenticationSchemes = "vcms", Roles = "Admin,Editor")]
    public class MemberVcmsController(UnitOfWork unitOfWork) : Controller
    {
        public async Task<IActionResult> ListMember(
            int? page, 
            string? term = "",
            string? email = "",
            string? mobile = ""
        )
        {
            ViewBag.Message = TempData["Message"];

            var list = unitOfWork.Member.GetPagedListAsync(
                    predicate: m => m.FullName.Contains(term)
                                    && m.Email.Contains(term)
                                    && m.PhoneNumber.Contains(term),
                    pageIndex: page ?? 1,
                    pageSize: 10
                );

            var model = new MemberVcmsViewModel
            {
                ListMember = list,
                Term = term,
                Email = email,
                Mobile = mobile
            };
            return View(model);
        }

        public IActionResult Member()
        {
            var model = new MemberVcmsViewModel
            {
                Member = new Models.Member()
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Member(MemberVcmsViewModel model)
        {
            var exists = unitOfWork.Member.GetAll(
                predicate: m => m.Email == model.Member.Email
            ).Any();

            if (exists)
            {
                ModelState.AddModelError("", "Địa chỉ email đã được sử dụng.");
                return View(model);
            }

            await unitOfWork.Member.InsertAsync(model.Member);
            await unitOfWork.CommitAsync();
            
            TempData["Message"] = "success|Thêm mới thành viên thành công!";
            return RedirectToAction(nameof(ListMember));
        }

        public async Task<IActionResult> UpdateMember(int id)
        {
            var member = await unitOfWork.Member.FindAsync(id);

            if (member == null) return NotFound();

            member.Password = null;

            var model = new MemberVcmsViewModel
            {
                Member = member
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMember(MemberVcmsViewModel model)
        {
            var exists = unitOfWork.Member.GetAll(
                predicate: m => m.Email == model.Member.Email && m.Id != model.Member.Id
            ).Any();

            if (exists)
            {
                ModelState.AddModelError("", "Địa chỉ email đã được sử dụng.");
                return View(model);
            }

            if (model.Password != null)
            {
                model.Member.Password = HtmlHelpers.ComputeHash(model.Password, Environment.GetEnvironmentVariable("vico@123"), 3);
            }
            else
            {
                var password = await unitOfWork.Member.GetAll(
                    predicate: m => m.Id == model.Member.Id,
                    selector: m => m.Password
                ).FirstOrDefaultAsync();

                model.Member.Password = password;
            }

            unitOfWork.Member.Update(model.Member);
            await unitOfWork.CommitAsync();
            
            TempData["Message"] = "success|Cập nhật thành công!";
            return RedirectToAction(nameof(ListMember));
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await unitOfWork.Member.FindAsync(id);

            if (member == null) return Json(new { success = false });

            unitOfWork.Member.Delete(member);
            await unitOfWork.CommitAsync();

            return Json(new { success = true });
        }
    }
}

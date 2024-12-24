using Evarosa.Data;
using Evarosa.Models;
using Evarosa.Services;
using Evarosa.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace NamTinNgoc.Controllers
{
    [Authorize(AuthenticationSchemes = "vcms", Roles = "Admin,Editor")]
    public class BannerController(UnitOfWork unitOfWork, IFileService fileService) : Controller
    {
        #region Banner
        public IActionResult ListBanner(int? page, int groupId = 0, string result = "")
        {
            ViewBag.Banner = result;
            var pageNumber = page ?? 1;
            var banners = unitOfWork.Banner.GetAll();

            if (groupId > 0)
            {
                banners = banners.Where(a => a.GroupId == groupId);
            }

            var model = new ListBannerViewModel
            {
                Banners = banners.OrderBy(m => m.Sort).ToPagedList(pageNumber, 10),
            };
            return View(model);
        }

        public IActionResult Banner()
        {
            var model = new BannerViewModel
            {
                Banner = new Banner()
                {
                    Sort = 1,
                    Active = true,
                }
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Banner(BannerViewModel model)
        {
            model.Banner.Image = model.Image;
            await unitOfWork.Banner.InsertAsync(model.Banner);
            await unitOfWork.CommitAsync();
            return RedirectToAction(nameof(ListBanner), new { result = "success" });
        }

        public async Task<IActionResult> EditBanner(int bannerId = 0)
        {
            var banner = await unitOfWork.Banner.FindAsync(bannerId);
            if (banner == null)
            {
                return RedirectToAction(nameof(ListBanner));
            }
            var model = new BannerViewModel
            {
                Banner = banner,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditBanner(BannerViewModel model)
        {
            var banner = await unitOfWork.Banner.FindAsync(model.Banner.Id);

            if (banner == null) return RedirectToAction("ListBanner");

            banner.Image = model.Image;
            banner.GroupId = model.Banner.GroupId;
            banner.Name = model.Banner.Name;
            banner.Slogan = model.Banner.Slogan;
            banner.Sort = model.Banner.Sort;
            banner.Active = model.Banner.Active;
            banner.Url = model.Banner.Url;
            banner.Content = model.Banner.Content;

            unitOfWork.Banner.Update(banner);
            await unitOfWork.CommitAsync();
            return RedirectToAction("ListBanner", new { result = "update" });
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<bool> DeleteBanner(int bannerId = 0)
        {
            var banner = await unitOfWork.Banner.FindAsync(bannerId);

            if (banner == null)
            {
                return false;
            }

            var user = await unitOfWork.Admin.GetAll(
                    predicate: m => m.Username == User.Identity.Name
                ).FirstOrDefaultAsync();

            unitOfWork.Banner.Delete(banner);
            await unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> UpdateBannerQuick(int sort = 1, bool active = false, int bannerId = 0)
        {
            var banner = await unitOfWork.Banner.FindAsync(bannerId);

            if (banner == null)
            {
                return false;
            }

            var user = await unitOfWork.Admin.GetAll(
                    predicate: m => m.Username == User.Identity.Name
                ).FirstOrDefaultAsync();

            banner.Sort = sort;
            banner.Active = active;

            unitOfWork.Banner.Update(banner);
            unitOfWork.Commit();
            return true;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Rollback();
            base.Dispose(disposing);
        }
    }
}

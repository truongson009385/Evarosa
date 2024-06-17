using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Evarosa.Data;
using Evarosa.Models;
using Evarosa.Services;
using Evarosa.Utils;
using Evarosa.ViewModels;
using X.PagedList;

namespace Evarosa.Controllers
{
    [Authorize(AuthenticationSchemes = "vcms", Roles = "Admin,Editor")]
    public class ProductVcmsController(UnitOfWork unitOfWork, IFileService fileService) : Controller
    {

        #region ProductCategory
        public async Task<IActionResult> ListProductCategory(int? page, string term = "")
        {
            ViewBag.Message = TempData["Message"];

            int pageNumber = page ?? 1;

            var list = await unitOfWork.ProductCategory
                .GetPagedListAsync(
                    predicate: m => m.ParentCategoryId == null && m.Title.Contains(term),
                    include: m => m.Include(l => l.CategoryChildren).ThenInclude(l => l.CategoryChildren),
                    orderBy: m => m.OrderBy(l => l.Sort),
                    pageIndex: pageNumber,
                    pageSize: 10
                );

            var model = new ProductCategoryViewModel
            {
                ListProductCategory = list,
                Term = term,
            };
            return View(model);
        }

        public async Task<IActionResult> ProductCategory()
        {
            var banners = await unitOfWork.Banner.GetAllAsync(
                    predicate: m => m.GroupId == 4,
                    orderBy: m => m.OrderByDescending(l => l.Sort)
                );

            ViewData["banners"] = new SelectList(banners, "Id", "Name");
            ViewData["categories"] = new SelectList(await GetItemsSelectCategory(), "Id", "Title");

            var model = new ProductCategoryViewModel
            {

            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductCategory(ProductCategoryViewModel model)
        {
            model.ProductCategory.Url = HtmlHelpers.ConvertToUnSign(model.Url ?? model.ProductCategory.Title);
            unitOfWork.ProductCategory.Insert(model.ProductCategory);
            await unitOfWork.CommitAsync();
            var count = await unitOfWork.ProductCategory.CountAsync(m => m.Url == model.ProductCategory.Url);
            if (count > 1)
            {
                model.ProductCategory.Url += "-" + model.ProductCategory.Id;
                await unitOfWork.CommitAsync();
            }

            TempData["Message"] = "success|Thêm mới thành công danh mục sản phẩm";
            return RedirectToAction("ListProductCategory");
        }

        public async Task<IActionResult> UpdateProductCategory(int id)
        {
            var category = await unitOfWork.ProductCategory.FindAsync(id);

            if (category == null) return NotFound();

            var banners = await unitOfWork.Banner.GetAllAsync(
                    predicate: m => m.GroupId == 4,
                    orderBy: m => m.OrderByDescending(l => l.Sort)
                );

            ViewData["banners"] = new SelectList(banners, "Id", "Name");
            ViewData["categories"] = new SelectList(await GetItemsSelectCategory(id), "Id", "Title");

            var model = new ProductCategoryViewModel
            {
                ProductCategory = category,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductCategory(ProductCategoryViewModel model)
        {
            model.ProductCategory.Url = HtmlHelpers.ConvertToUnSign(model.Url ?? model.ProductCategory.Title);

            unitOfWork.ProductCategory.UpdateAsync(model.ProductCategory);
            await unitOfWork.CommitAsync();
            var count = await unitOfWork.ProductCategory.CountAsync(m => m.Url == model.ProductCategory.Url);
            if (count > 1)
            {
                model.ProductCategory.Url += "-" + model.ProductCategory.Id;
                await unitOfWork.CommitAsync();
            }

            TempData["Message"] = "success|Cập nhật thành công danh mục sản phẩm";
            return RedirectToAction("ListProductCategory");
        }

        [HttpPost]
        public async Task<IActionResult> QuickUpdateCategory(int categoryId, int sort, bool showHome, bool showMenu, bool showHeader, bool showOutstanding, bool display, bool isActive)
        {
            var item = await unitOfWork.ProductCategory.FindAsync(categoryId);

            if (item == null) return Json(new { success = false });

            item.Sort = sort;
            item.ShowHome = showHome;
            item.ShowMenu = showMenu;
            item.ShowHeader = showHeader;
            item.ShowOutstanding = showOutstanding;
            item.Display = display;
            item.Active = isActive;

            await unitOfWork.CommitAsync();
            return Json(new { success = true });
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            var category = await unitOfWork.ProductCategory.FindAsync(id);

            if (category == null) return Json(new { success = false });

            unitOfWork.ProductCategory.Delete(category);
            await unitOfWork.CommitAsync();
            return Json(new { success = true });
        }
        #endregion

        #region Product
        public async Task<IActionResult> ListProduct(int? page, DateTime? timer, int? instock, string? masp, int? cateId, string term = "")
        {
            ViewBag.Message = TempData["Message"];

            int pageNumber = page ?? 1;

            var list = unitOfWork.Product
                .GetAll(
                    predicate: m => 
                        m.Name.Contains(term),
                    include: l => l.Include(m => m.ProductCategory),
                    orderBy: m => m.OrderByDescending(l => l.Sort)
                );

            if (cateId.HasValue)
            {
                list = list.Where(m => m.ProductCategoryId == cateId);
            }

            if (timer.HasValue)
            {
                list = list.Where(m => m.CreatedAt > timer);
            }

            if (instock.HasValue)
            {
                list = list.Where(m => m.InStock == instock);
            }

            if (!string.IsNullOrEmpty(masp))
            {
                list = list.Where(m => m.MaSP == masp);
            }

            var model = new ProductViewModel
            {
                ListProduct = await list.ToPagedListAsync(pageNumber, 10),
                Term = term,
                CategoryId = cateId,
                Timer = timer,
                InStock = instock,
                MaSP = masp
            };
            return View(model);
        }

        public async Task<IActionResult> Product()
        {
            ViewData["categories"] = new SelectList(await GetItemsSelectCategory(), "Id", "Title");

            var products = unitOfWork.Product.GetAll();
            var sttMax = products.Any() ? products.Max(m => m.Sort) : 1;

            var model = new ProductViewModel
            {
                Product = new Product
                {
                    Sort = sttMax + 1,
                },
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Product(ProductViewModel model, int[] items, string[] values)
        {
            var user = await unitOfWork.Admin.GetAll(
                    predicate: m => m.Username == User.Identity.Name
                ).FirstOrDefaultAsync();

            // Price Conversion
            if (model.Price != null)
            {
                model.Product.Price = Convert.ToDecimal(model.Price.Replace(",", ""));
            }
            if (model.PriceSale != null)
            {
                model.Product.PriceSale = Convert.ToDecimal(model.PriceSale.Replace(",", ""));
            }

            // URL Generation
            model.Product.Url = HtmlHelpers.ConvertToUnSign(model.Url ?? model.Product.Name);

            var count = unitOfWork.Product.Count(m => m.Url == model.Product.Url);
            if (count > 0)
            {
                model.Product.Url += "-" + DateTime.Now.Millisecond;
            }

            unitOfWork.Product.Insert(model.Product);
            await unitOfWork.CommitAsync();

            TempData["Message"] = "success|Thêm mới thành công sản phẩm";
            return RedirectToAction("ListProduct");
        }


        public async Task<IActionResult> UpdateProduct(int id)
        {
            var product = await unitOfWork.Product.GetAll(
                predicate: m => m.Id == id,
                include: l => l.Include(m => m.ProductCategory)
            ).FirstOrDefaultAsync();

            if (product == null) return NotFound();

            ViewData["categories"] = new SelectList(await GetItemsSelectCategory(), "Id", "Title");

            var model = new ProductViewModel
            {
                Product = product,
                Price = product.Price.ToString("N0"),
                PriceSale = product.PriceSale.ToString("N0"),
                Url = product.Url,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductViewModel model, int[] items, string[] values)
        {
            var product = await unitOfWork.Product.GetAll(
                    predicate: m => m.Id == model.Product.Id,
                    include: l => l.Include(m => m.ProductCategory),
                    disableTracking: false
                ).FirstOrDefaultAsync();

            if (product == null) return NotFound();

            var user = await unitOfWork.Admin.GetAll(
                    predicate: m => m.Username == User.Identity.Name
                ).FirstOrDefaultAsync();

            // Price Conversion
            if (model.Price != null)
            {
                product.Price = Convert.ToDecimal(model.Price.Replace(",", ""));
            }
            else
            {
                product.Price = decimal.Zero;
            }

            if (model.PriceSale != null)
            {
                product.PriceSale = Convert.ToDecimal(model.PriceSale.Replace(",", ""));
            }
            else
            {
                product.PriceSale = decimal.Zero;
            }

            product.Url = HtmlHelpers.ConvertToUnSign(model.Url ?? model.Product.Name);

            var slugExists = unitOfWork.Product.Count(m => m.Url == model.Product.Url && m.Id != model.Product.Id) > 0;
            if (slugExists)
            {
                product.Url += $"-{DateTime.Now.Millisecond}";
            }

            product.Title = model.Product.Title;
            product.Description = model.Product.Description;
            product.Name = model.Product.Name;
            product.MaSP = model.Product.MaSP;
            product.ShortDescription = model.Product.ShortDescription;
            product.Images = model.Product.Images;
            product.Active = model.Product.Active;
            product.ShowOutstanding = model.Product.ShowOutstanding;
            product.Content = model.Product.Content;
            product.InStock = model.Product.InStock;
            product.Sort = model.Product.Sort;
            product.ProductCategoryId = model.Product.ProductCategoryId;

            unitOfWork.Product.Update(product);
            await unitOfWork.CommitAsync();

            TempData["Message"] = "success|Cập nhật thành công sản phẩm";
            return RedirectToAction("ListProduct");
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var item = await unitOfWork.Product.FindAsync(id);

            if (item == null) return Json(new { success = false });

            var user = await unitOfWork.Admin.GetAll(
                   predicate: m => m.Username == User.Identity.Name
               ).FirstOrDefaultAsync();

            unitOfWork.Product.Delete(item);
            await unitOfWork.CommitAsync();
            return Json(new { success = true });
        }
        #endregion

        #region Func
        private async Task<IEnumerable<ProductCategory>> GetItemsSelectCategory(int id = 0)
        {
            var query = unitOfWork.ProductCategory.GetAll(predicate: c => c.ParentCategoryId == null);

            if (id != 0)
            {
                query = query.Where(c => c.Id != id);
            }

            var items = await query.Include(c => c.CategoryChildren).ThenInclude(c => c.CategoryChildren).ToListAsync();

            var resultitems = new List<ProductCategory>();

            Func<List<ProductCategory>, int, List<ProductCategory>>? changeTitleCategory = null;
            changeTitleCategory = (categories, level) =>
            {
                var modifiedCategories = new List<ProductCategory>();
                string prefix = String.Concat(Enumerable.Repeat("—", level));
                foreach (var category in categories)
                {
                    var modifiedCategory = new ProductCategory
                    {
                        Id = category.Id,
                        Title = prefix + " " + category.Title,
                    };
                    modifiedCategories.Add(modifiedCategory);

                    if (category.CategoryChildren is { Count: > 0 })
                    {
                        var modifiedChildren = changeTitleCategory?.Invoke(category.CategoryChildren.ToList(), level + 1);
                        modifiedCategories.AddRange(modifiedChildren);
                    }
                }
                return modifiedCategories;
            };

            var modifiedItems = changeTitleCategory(items, 0);
            resultitems.AddRange(modifiedItems);

            return resultitems;
        }


        #endregion

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Rollback();
            base.Dispose(disposing);
        }
    }
}

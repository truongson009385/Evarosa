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
            model.ProductCategory.Image = model.Image;
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
            model.ProductCategory.Image = model.Image;
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
                    include: l => l
                        .Include(m => m.ProductCategory)
                        .Include(m => m.Skus),
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
            var options = unitOfWork.Option.GetAll();

            var model = new ProductViewModel
            {
                Product = new Product
                {
                    Sort = sttMax + 1,
                },
                Options = options
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Product(ProductViewModel model)
        {
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
            model.Product.Images = string.Join(",", model.Images);

            var count = unitOfWork.Product.Count(m => m.Url == model.Product.Url);
            if (count > 0)
            {
                model.Product.Url += "-" + DateTime.Now.Millisecond;
            }

            unitOfWork.Product.Insert(model.Product);
            await unitOfWork.CommitAsync();

            if (model.SkuProduct is { Indexs: > 0 })
            {
                for (int i = 0; i < model.SkuProduct.Indexs; i++)
                {
                    var sku = new Sku
                    {
                        ProductId = model.Product.Id,
                        SKU = model.SkuProduct.Skus[i],
                        InStock = model.SkuProduct.Stocks[i],
                        Price = model.SkuProduct.Prices[i] != null ? Convert.ToDecimal(model.SkuProduct.Prices[i].Replace(",", "")) : 0,
                        PriceSale = model.SkuProduct.PriceSales[i] != null ? Convert.ToDecimal(model.SkuProduct.PriceSales[i].Replace(",", "")) : 0,
                    };

                    var options = model.SkuProduct.Options[i].Split("/");
                    var values = model.SkuProduct.Values[i].Split("/");

                    unitOfWork.Sku.Insert(sku);
                    await unitOfWork.CommitAsync();

                    for (int j = 0; j < options.Length; j++)
                    {
                        var optionSku = new OptionSku
                        {
                            SkuId = sku.Id,
                            OptionId = Convert.ToInt32(options[j]),
                            Value = values[j],
                        };

                        unitOfWork.OptionSku.Insert(optionSku);
                    }
                }

                await unitOfWork.CommitAsync();
            }

            
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

            var options = unitOfWork.Option.GetAll();
            var skuQr = unitOfWork.Sku.GetAll(predicate: o => o.ProductId == product.Id);

            var optionProductVcms = await skuQr.SelectMany(o => o.OptionSkus)
                .GroupBy(optionSku => optionSku.Option.Id)
                .Select(group => new OptionProductVcms
                {
                    Id = group.Key,
                    Options = string.Join(",", group.Select(optionSku => optionSku.Value).Distinct().ToList())
                })
                .FirstOrDefaultAsync();

            var ids = await unitOfWork.OptionSku.GetAll(
                    predicate: m => skuQr.Any(o => o.Id == m.SkuId)
                ).Select(m => m.OptionId).Distinct().ToArrayAsync();

            var values = await skuQr.SelectMany(o => o.OptionSkus)
                .Select(optionSku => optionSku.Value)
                .Distinct()
                .ToListAsync();

            var attrs = new List<string> { string.Join(",", values) };
            var items = GenerateCombinations(attrs.ToArray(), 0, new string[attrs.Count]);

            var skuP = new SkuProductViewModel
            {
                OptionIds = ids,
                Attrs = items,
                FirstItem = attrs.FirstOrDefault(),
                Ids = await skuQr.Select(m => m.Id).ToArrayAsync(),
                Skus = await skuQr.Select(m => m.SKU ?? "").ToArrayAsync(),
                Stocks = await skuQr.Select(m => m.InStock).ToArrayAsync(),
                Prices = await skuQr.Select(m => m.Price.ToString("N0")).ToArrayAsync(),
                PriceSales = await skuQr.Select(m => m.PriceSale.ToString("N0")).ToArrayAsync(),
            };

            var model = new ProductViewModel
            {
                Product = product,
                Price = product.Price.ToString("N0"),
                PriceSale = product.PriceSale.ToString("N0"),
                Url = product.Url,
                SkuProduct = skuP,
                Options = options,
                OptionProductVcms = optionProductVcms,
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductViewModel model)
        {
            var product = await unitOfWork.Product.GetAll(
                    predicate: m => m.Id == model.Product.Id,
                    include: l => l.Include(m => m.ProductCategory),
                    disableTracking: false
                ).FirstOrDefaultAsync();

            if (product == null) return NotFound();

            // Price Conversion
            product.Price = model.Price != null ? Convert.ToDecimal(model.Price.Replace(",", "")) : decimal.Zero;
            product.PriceSale = model.PriceSale != null ? Convert.ToDecimal(model.PriceSale.Replace(",", "")) : decimal.Zero;

            product.Url = HtmlHelpers.ConvertToUnSign(model.Url ?? model.Product.Name);

            var slugExists = unitOfWork.Product.Count(m => m.Url == model.Product.Url && m.Id != model.Product.Id) > 0;
            if (slugExists)
            {
                product.Url += $"-{DateTime.Now.Millisecond}";
            }

            product.Images = string.Join(",", model.Images);
            product.Title = model.Product.Title;
            product.Description = model.Product.Description;
            product.Name = model.Product.Name;
            product.MaSP = model.Product.MaSP;
            product.ShortDescription = model.Product.ShortDescription;
            product.Active = model.Product.Active;
            product.ShowOutstanding = model.Product.ShowOutstanding;
            product.Content = model.Product.Content;
            product.InStock = model.Product.InStock;
            product.IsOrder = model.Product.IsOrder;
            product.Sort = model.Product.Sort;
            product.ProductCategoryId = model.Product.ProductCategoryId;

            unitOfWork.Product.Update(product);

            var skuList = new List<Sku>();

            if (model.SkuProduct is { Indexs: > 0 })
            {
                for (int i = 0; i < model.SkuProduct.Indexs; i++)
                {
                    var sku = new Sku
                    {
                        Id = model.SkuProduct.Ids.ElementAtOrDefault(i),
                        ProductId = model.Product.Id,
                        SKU = model.SkuProduct.Skus.ElementAtOrDefault(i),
                        InStock = model.SkuProduct.Stocks.ElementAtOrDefault(i),
                        Price = model.SkuProduct.Prices?.ElementAtOrDefault(i) != null ? Convert.ToDecimal(model.SkuProduct.Prices.ElementAtOrDefault(i)?.Replace(",", "")) : 0,
                        PriceSale = model.SkuProduct.PriceSales?.ElementAtOrDefault(i) != null ? Convert.ToDecimal(model.SkuProduct.PriceSales.ElementAtOrDefault(i)?.Replace(",", "")) : 0,
                    };

                    skuList.Add(sku);
                }

                unitOfWork.Sku.Update(skuList);
                await unitOfWork.CommitAsync();

                int index = 0;

                foreach (var sku in skuList)
                {
                    var options = model.SkuProduct.Options[index].Split("/");
                    var valuesj = model.SkuProduct.Values[index].Split("/");

                    for (int j = 0; j < options.Length; j++)
                    {
                        var optionSku = await unitOfWork.OptionSku.GetAll(
                                predicate: m => m.OptionId == Convert.ToInt32(options[j]) && m.SkuId == sku.Id,
                                disableTracking: false
                            ).FirstOrDefaultAsync();

                        if (optionSku != null)
                        {
                            optionSku.Value = valuesj[j];
                            unitOfWork.OptionSku.Update(optionSku);
                        }
                        else
                        {
                            optionSku = new OptionSku
                            {
                                OptionId = Convert.ToInt32(options[j]),
                                SkuId = sku.Id,
                                Value = valuesj[j],
                            };
                            unitOfWork.OptionSku.Insert(optionSku);
                        }

                        await unitOfWork.CommitAsync();
                    }

                    index++;
                }
            }

            var skusToDelete = await unitOfWork.Sku.GetAllAsync(predicate: sku => !skuList.Select(a => a.Id).Contains(sku.Id) && sku.ProductId == product.Id);

            unitOfWork.Sku.Delete(skusToDelete);
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

        public IActionResult ListSku(string[] attrs, int[] ids)
        {
            List<string> items = GenerateCombinations(attrs, 0, new string[attrs.Length]);

            var skuP = new SkuProductViewModel
            {
                OptionIds = ids,
                Attrs = items,
                FirstItem = attrs[0],
            };

            var model = new ProductViewModel { SkuProduct = skuP };
            return PartialView(model);
        }
        #endregion

        #region Option
        public IActionResult ListOption(int? page, string result = "")
        {
            ViewBag.Banner = result;
            var pageNumber = page ?? 1;
            var options = unitOfWork.Option.GetAll(
                    orderBy: m => m.OrderByDescending(a => a.Sort)
                );

            var model = new OptionViewModel
            {
                ListOption = options.OrderBy(m => m.Sort).ToPagedList(pageNumber, 10),
            };
            return View(model);
        }

        public IActionResult Option()
        {
            var model = new OptionViewModel
            {
                Option = new Option()
                {
                    Sort = 1,
                }
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Option(OptionViewModel model)
        {
            await unitOfWork.Option.InsertAsync(model.Option);
            await unitOfWork.CommitAsync();
            return RedirectToAction(nameof(ListOption), new { result = "success" });
        }

        public async Task<IActionResult> UpdateOption(int id)
        {
            var option = await unitOfWork.Option.FindAsync(id);
            if (option == null)
            {
                return RedirectToAction(nameof(ListOption));
            }
            var model = new OptionViewModel
            {
                Option = option,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOption(OptionViewModel model)
        {
            var option = await unitOfWork.Option.FindAsync(model.Option.Id);

            if (option == null) return RedirectToAction(nameof(ListOption));

            option.Name = model.Option.Name;
            option.Sort = model.Option.Sort;

            unitOfWork.Option.Update(option);
            await unitOfWork.CommitAsync();
            return RedirectToAction(nameof(ListOption), new { result = "update" });
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<bool> DeleteOption(int id)
        {
            var option = await unitOfWork.Option.FindAsync(id);

            if (option == null)
            {
                return false;
            }

            unitOfWork.Option.Delete(option);
            await unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> UpdateOptionQuick(int id, int sort = 1)
        {
            var option = await unitOfWork.Option.FindAsync(id);

            if (option == null)
            {
                return false;
            }

            option.Sort = sort;

            unitOfWork.Option.Update(option);
            unitOfWork.Commit();
            return true;
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

        private List<string> GenerateCombinations(string[] inputArray, int currentIndex, string[] currentCombination)
        {
            List<string> resultList = new List<string>();

            if (currentIndex == inputArray.Length)
            {
                resultList.Add(string.Join("/", currentCombination));
            }
            else
            {
                string[] parts = inputArray[currentIndex].Split(',');

                foreach (string part in parts)
                {
                    currentCombination[currentIndex] = part;
                    resultList.AddRange(GenerateCombinations(inputArray, currentIndex + 1, currentCombination));
                }
            }

            return resultList;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Rollback();
            base.Dispose(disposing);
        }
    }
}

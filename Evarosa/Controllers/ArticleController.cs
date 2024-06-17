using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Evarosa.Data;
using Evarosa.Models;
using Evarosa.Services;
using Evarosa.Utils;
using Evarosa.ViewModels;
using System.Security.Claims;
using X.PagedList;

namespace Evarosa.Controllers
{
    [Authorize(AuthenticationSchemes = "vcms", Roles = "Admin,Editor,CopyWriter")]
    public class ArticleController(UnitOfWork unitOfWork, IFileService fileService) : Controller
    {
        #region ArticleCategory
        public async Task<IActionResult> ListArticleCategory(int? page, string? term = "")
        {
            int pageNumber = page ?? 1;

            var list = await unitOfWork.ArticleCategory
                .GetPagedListAsync(
                    predicate: m => m.ParentCategoryId == null && m.Title.Contains(term),
                    include: m => m.Include(l => l.CategoryChildren)
                        .ThenInclude(l => l.CategoryChildren),
                    orderBy: m => m.OrderBy(l => l.Sort),
                    pageIndex: pageNumber,
                    pageSize: 10
                );

            var model = new ArticleCategoryViewModel
            {
                ListArticleCategory = list,
            };
            return View(model);
        }

        public async Task<IActionResult> ArticleCategory()
        {
            var model = new ArticleCategoryViewModel
            {
                SelectListCategory = await GetCategorySelectAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ArticleCategory(ArticleCategoryViewModel model)
        {
            model.ArticleCategory.Url = HtmlHelpers.ConvertToUnSign(model.Url ?? model.ArticleCategory.Title);


            await unitOfWork.ArticleCategory.InsertAsync(model.ArticleCategory);
            await unitOfWork.CommitAsync();

            var count = await unitOfWork.ArticleCategory.CountAsync(m => m.Url == model.ArticleCategory.Url);
            if (count > 1)
            {
                model.ArticleCategory.Url += "-" + model.ArticleCategory.Id;
                await unitOfWork.CommitAsync();
            }

            TempData["message"] = $"success|Tạo thành công danh mục bài viết {model.ArticleCategory.Title}.";
            return RedirectToAction("ListArticleCategory");
        }

        public async Task<IActionResult> UpdateArticleCategory(int id)
        {
            var category = await unitOfWork.ArticleCategory.FindAsync(id);

            if (category == null) return NotFound();

            var model = new ArticleCategoryViewModel
            {
                ArticleCategory = category,
                Url = category.Url,
                SelectListCategory = await GetCategorySelectAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateArticleCategory(ArticleCategoryViewModel model)
        {
            model.ArticleCategory.Url = HtmlHelpers.ConvertToUnSign(model.Url ?? model.ArticleCategory.Title);

            unitOfWork.ArticleCategory.UpdateAsync(model.ArticleCategory);
            await unitOfWork.CommitAsync();
            var count = await unitOfWork.ArticleCategory.CountAsync(m => m.Url == model.ArticleCategory.Url);
            if (count > 1)
            {
                model.ArticleCategory.Url += "-" + model.ArticleCategory.Id;
                await unitOfWork.CommitAsync();
            }
            TempData["message"] = $"success|Tạo thành công danh mục bài viết {model.ArticleCategory.Title}.";
            return RedirectToAction("ListArticleCategory");
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteArticleCategory(int id)
        {
            var category = await unitOfWork.ArticleCategory.FindAsync(id);

            if (category == null) return Json(new { success = false });

            unitOfWork.ArticleCategory.Delete(category);
            await unitOfWork.CommitAsync();
            return Json(new { success = true });
        }
        #endregion

        #region Article
        public async Task<IActionResult> ListArticle(int? page, string? term, TypeArticle? type, int? cateId, DateTime? timer, string result = "")
        {
            ViewBag.Result = result;

            int pageNumber = page ?? 1;

            var qrArticles = unitOfWork.Article
                .GetAll(
                    include: m => m.Include(l => l.ArticleCategory),
                    orderBy: o => o.OrderByDescending(l => l.Sort)
                );

            if (!string.IsNullOrEmpty(term))
            {
                qrArticles = qrArticles.Where(m => m.Name.Contains(term));
            }

            if (type.HasValue)
            {
                qrArticles = qrArticles.Where(m => m.ArticleCategory.Type == type);
            }

            if (cateId.HasValue)
            {
                qrArticles = qrArticles.Where(m => m.ArticleCategory.Id == cateId);
            }

            if (timer.HasValue)
            {
                qrArticles = qrArticles.Where(m => m.CreatedAt > timer);
            }

            var model = new ArticleViewModel
            {
                ListArticle = qrArticles.ToPagedList(pageNumber, 5),
                TypeArticle = type,
                CategoryId = cateId,
                Term = term,
                Timer = timer,
                SelectListCategory = await GetCategorySelectAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> Article()
        {
            var articles = unitOfWork.Article.GetAll();
            var sttMax = articles.Any() ? articles.Max(m => m.Sort) : 1;

            var model = new ArticleViewModel
            {
                Article = new Article
                {
                    Sort = sttMax + 1,
                },
                SelectListCategory = await GetCategorySelectAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Article(ArticleViewModel model)
        {
            var user = await unitOfWork.Admin.GetAll(
                    predicate: m => m.Username == User.Identity.Name
                ).FirstOrDefaultAsync();

            model.Article.Url = HtmlHelpers.ConvertToUnSign(model.Url ?? model.Article.Name);
            var count = await unitOfWork.Article.CountAsync(m => m.Url == model.Article.Url);
            
            if (count > 0)
            {
                model.Article.Url += "-" + DateTime.Now.Millisecond;
            }

            await unitOfWork.Article.InsertAsync(model.Article);
            await unitOfWork.CommitAsync();
            
            TempData["message"] = $"success|Tạo thành công bài viết {model.Article.Name}.";
            return RedirectToAction("ListArticle");
        }

        public async Task<IActionResult> UpdateArticle(int id)
        {
            var article = await unitOfWork.Article
                .GetAll(
                    predicate: m => m.Id == id,
                    include: m => m.Include(l => l.ArticleCategory)
                ).FirstOrDefaultAsync();

            if (article == null) return NotFound();

            var model = new ArticleViewModel
            {
                Article = article,
                Url = article.Url,
                SelectListCategory = await GetCategorySelectAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateArticle(ArticleViewModel model)
        {
            var article = await unitOfWork.Article
                .GetAll(
                    predicate: m => m.Id == model.Article.Id,
                    include: m => m.Include(l => l.ArticleCategory),
                    disableTracking: false
                ).FirstOrDefaultAsync();


            if (article == null) return NotFound();
            
            var user = await unitOfWork.Admin.GetAll(
                    predicate: m => m.Username == User.Identity.Name
                ).FirstOrDefaultAsync();

            article.Url = HtmlHelpers.ConvertToUnSign(model.Url ?? model.Article.Name);
            var count = await unitOfWork.Article.GetAll(predicate: m => m.Url == article.Url && m.Id != article.Id).CountAsync();
            if (count > 0)
            {
                article.Url += "-" + DateTime.Now.Millisecond;
            }

            article.TitleMeta = model.Article.TitleMeta;
            article.DescriptionMeta = model.Article.DescriptionMeta;
            article.ArticleCategoryId = model.Article.ArticleCategoryId;
            article.ShortDescription = model.Article.ShortDescription;
            article.Body = model.Article.Body;
            article.Image = model.Article.Image;
            article.Name = model.Article.Name;
            article.Active = model.Article.Active;
            article.ShowHome = model.Article.ShowHome;
            article.Sort = model.Article.Sort;

            await unitOfWork.CommitAsync();

            TempData["message"] = $"success|Cập nhật thành công bài viết {model.Article.Name}.";
            return RedirectToAction("ListArticle");
        }

        [HttpPost, Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await unitOfWork.Article.FindAsync(id);

            if (article == null) return Json(new { success = false });

            unitOfWork.Article.Delete(article);
            await unitOfWork.CommitAsync();

            return Json(new { success = true });
        }
        #endregion

        #region Func
        private async Task<SelectList> GetCategorySelectAsync(int id = 0)
        {
            var items = await unitOfWork.ArticleCategory
                                .GetAllAsync(
                                    predicate: c => c.ParentCategoryId == null && c.Id != id,
                                    include: m => m.Include(c => c.CategoryChildren).ThenInclude(c => c.CategoryChildren)
                                );

            var resultItems = new List<SelectListItem>();

            resultItems.Add(new SelectListItem
            {
                Value = null,
                Text = "Chọn danh mục"
            });

            void ModifyCategoryTitles(IList<ArticleCategory> categories, string prefix = "")
            {
                foreach (var category in categories)
                {
                    resultItems.Add(new SelectListItem
                    {
                        Value = category.Id.ToString(),
                        Text = prefix + " " + category.Title
                    });

                    if (category.CategoryChildren?.Count > 0)
                    {
                        ModifyCategoryTitles(category.CategoryChildren.ToList(), prefix + "—");
                    }
                }
            }

            ModifyCategoryTitles(items);

            return new SelectList(resultItems, "Value", "Text");
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Rollback();
            base.Dispose(disposing);
        }
    }
}

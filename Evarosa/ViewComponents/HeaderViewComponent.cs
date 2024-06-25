using Evarosa.Data;
using Evarosa.Models;
using Evarosa.Services.Impl;
using Evarosa.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Evarosa.ViewComponents
{
    public class HeaderViewComponent(UnitOfWork unitOfWork) : ViewComponent
    {
        public ShoppingService cart => ShoppingService.GetCart(HttpContext, unitOfWork);

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new HeaderViewModel();

            //Query
            var qrCategoryArticle = unitOfWork.ArticleCategory
                .GetAll(
                    predicate: m => m.Active && m.ShowHeader,
                    orderBy: m => m.OrderByDescending(l => l.Sort)
                );

            var qrCategoryProduct = unitOfWork.ProductCategory
                .GetAll(
                    predicate: m => m.Active && m.ShowHeader,
                    orderBy: m => m.OrderByDescending(l => l.Sort)
                );

            model.ArticleCategories = qrCategoryArticle
                .Where(m => m.ParentCategoryId == null)
                .Select(m => new Models.ArticleCategory
                {
                    Title = m.Title,
                    Url = m.Url,
                    CategoryChildren = qrCategoryArticle
                        .Where(a => a.ParentCategoryId == m.Id)
                        .Select(a => new Models.ArticleCategory
                        {
                            Title = a.Title,
                            Url = a.Url,
                        })
                        .ToList()
                })
                .ToList();

            model.ProductCategories = qrCategoryProduct
                .Where(m => m.ParentCategoryId == null)
                .Select(m => new Models.ProductCategory
                {
                    Title = m.Title,
                    Url = m.Url,
                    CategoryChildren = qrCategoryProduct
                        .Where(a => a.ParentCategoryId == m.Id)
                        .Select(a => new Models.ProductCategory
                        {
                            Title = a.Title,
                            Url = a.Url,
                        })
                        .ToList()
                })
                .ToList();

            var categories = await unitOfWork.ProductCategory.GetAllAsync(
                    predicate: m => m.Active && m.ShowMenu,
                    selector: m => new { m.Id, m.Url, m.Title }
                );
            model.SelectCategories = new SelectList(categories, "Url", "Title");

            model.Count = cart.GetCount();
            model.CartMini = new CartMiniViewModel
            {
                CartItems = cart.GetCartItems()
            };

            return View(model);
        }
    }
}

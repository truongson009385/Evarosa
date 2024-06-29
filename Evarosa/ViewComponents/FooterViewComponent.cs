using Evarosa.Data;
using Evarosa.Models;
using Evarosa.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Evarosa.ViewComponents
{
    public class FooterViewComponent(UnitOfWork unitOfWork) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var qrArticle = unitOfWork.Article.GetAll(
                                predicate: a => a.Active && a.ShowFooter,
                                orderBy: a => a.OrderByDescending(m => m.Sort),
                                selector: a => new Article
                                {
                                    Id = a.Id,
                                    ArticleCategoryId = a.ArticleCategoryId,
                                    Name = a.Name,
                                    Url = a.Url
                                }
                            );
            var articleCategory = unitOfWork.ArticleCategory.GetAll(
                    predicate: m => m.Active && m.ShowFooter,
                    orderBy: m => m.OrderByDescending(l => l.Sort),
                    selector: m => new ArticleCategory
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Articles = qrArticle.Where(a => a.ArticleCategoryId == m.Id).Take(6).ToList()
                    }
                ).FirstOrDefault();

            var model = new FooterViewModel
            {
                ArticleCategory = articleCategory
            };
            return View(model);
        }
    }
}

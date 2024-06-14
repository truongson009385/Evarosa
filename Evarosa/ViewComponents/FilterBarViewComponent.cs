using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Evarosa.Data;
using Evarosa.Models;
using Evarosa.ViewModels;

namespace Evarosa.ViewComponents
{
    public class FilterBarViewComponent(UnitOfWork unitOfWork) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new FilterBarViewModel();

            model.ProductCategories = await unitOfWork.ProductCategory
                .GetAllAsync(
                    predicate: m => m.Active && m.ShowMenu && m.ParentCategoryId == null,
                    orderBy: m => m.OrderByDescending(l => l.Sort),
                    include: m => m
                        .Include(l => l.CategoryChildren.Take(12))
                        .ThenInclude(l => l.CategoryChildren.Take(12))
                );

            return View(model);
        }
    }
}

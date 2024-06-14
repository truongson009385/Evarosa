using Evarosa.Data;
using Evarosa.Models;
using Evarosa.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Evarosa.Controllers
{
    public class HomeController(UnitOfWork unitOfWork) : Controller
    {

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel();

            //Query
            var qrBanner = unitOfWork.Banner
                .GetAll(
                    predicate: m => m.Active,
                    orderBy: m => m.OrderByDescending(o => o.Sort)
            );

            var qrArticle = unitOfWork.Article
                .GetAll(
                    predicate: m => m.Active && m.ShowHome && m.ArticleCategory.Type == TypeArticle.TinTuc,
                    orderBy: m => m.OrderByDescending(o => o.Sort)
            );

            var qrProduct = unitOfWork.Product
                .GetAll(
                    predicate: m => m.Active && m.ShowHome,
                    orderBy: m => m.OrderByDescending(o => o.Sort)
            );

            var qrProductOutstanding = unitOfWork.Product
                .GetAll(
                    predicate: m => m.Active && m.ShowOutstanding,
                    orderBy: m => m.OrderByDescending(o => o.Sort)
            );


            //Get
            model.Banner = await qrBanner.Where(m => m.GroupId == 1).Take(20).ToListAsync();
            model.Partner = await qrBanner.Where(m => m.GroupId == 2).Take(30).ToListAsync();

            model.DisplayCategories = await unitOfWork.ProductCategory.GetAllAsync(
                    predicate: m => m.ParentCategoryId == null && m.Active && m.Display,
                    orderBy: m => m.OrderByDescending(o => o.Sort),
                    selector: m => new ProductCategory
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Url = m.Url,
                        Image = m.Image
                    }
                );

            model.ProductOutstanding = await unitOfWork.ProductCategory
                .GetAllAsync(
                    predicate: m => m.Active && m.ShowOutstanding,
                    orderBy: m => m.OrderBy(o => o.Sort),
                    selector: m => new ProductCategory
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Url = m.Url,
                        Products = qrProductOutstanding.Where(l => l.ProductCategoryId == m.Id).Take(20).ToList()
                    },
                    take: 8
                );

            model.ProductCategories = await unitOfWork.ProductCategory
                .GetAllAsync(
                    predicate: m => m.Active && m.ShowHome,
                    orderBy: m => m.OrderByDescending(o => o.Sort),
                    selector: m => new ProductCategory
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Url = m.Url,
                        Products = qrProduct.Where(l => l.ProductCategoryId == m.Id).Take(10).ToList()
                    },
                    take: 20
                );

            model.Articles = await qrArticle.Take(7).ToListAsync();

            return View(model);
        }


        [Route("{url}.html", Order = 0)]
        public async Task<IActionResult> ProductDetails(string url)
        {
            var product = await unitOfWork.Product
                .GetAll(
                    predicate: m => m.Url == url,
                    include: m => m.Include(l => l.ProductCategory),
                    disableTracking: false
                ).FirstOrDefaultAsync();

            if (product == null) return NotFound();

            var relatedProducts = await unitOfWork.Product
                .GetAllAsync(
                    predicate: m => m.ProductCategoryId == product.ProductCategoryId && m.Active && m.Id != product.Id,
                    selector: m => new Product
                    {
                        Name = m.Name,
                        Url = m.Url,
                        Images = m.Images,
                        Price = m.Price,
                        PriceSale = m.PriceSale,
                    },
                    take: 10,
                    orderBy: m => m.OrderByDescending(o => o.Sort)
                );

            var model = new PageProductViewModel
            {
                Product = product,
                Products = relatedProducts,
            };
            return View(model);
        }

        public IActionResult AllProduct()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

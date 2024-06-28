using Evarosa.Data;
using Evarosa.Models;
using Evarosa.Services;
using Evarosa.Services.Impl;
using Evarosa.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace Evarosa.Controllers
{
    public class HomeController(UnitOfWork unitOfWork, IMailService mailService, IAppService appService) : Controller
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
                    orderBy: m => m.OrderByDescending(o => o.Sort),
                    include: m => m.Include(l => l.Skus)
            );

            var qrProductOutstanding = unitOfWork.Product
                .GetAll(
                    predicate: m => m.Active && m.ShowOutstanding,
                    orderBy: m => m.OrderBy(o => Guid.NewGuid()),
                    include: m => m.Include(l => l.Skus)
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
                    orderBy: m => m.OrderByDescending(o => o.Sort),
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

        [Route("gioi-thieu")]
        public async Task<IActionResult> About()
        {
            var article = await unitOfWork.Article
                .GetAll(
                    predicate: m => m.ArticleCategory.Type == TypeArticle.GioiThieu && m.Active,
                    orderBy: m => m.OrderByDescending(o => o.Sort)
                ).FirstOrDefaultAsync();

            var model = new PageArticleViewModel
            {
                Article = article ?? new Article()
            };
            return View(model);
        }

        [Route("{url}.html", Order = 0)]
        public async Task<IActionResult> ProductDetails(string url)
        {
            var product = await unitOfWork.Product
                .GetAll(
                    predicate: m => m.Url == url,
                    include: m => m.Include(l => l.ProductCategory)
                        .Include(l => l.Skus),
                    disableTracking: false
                ).FirstOrDefaultAsync();

            if (product == null) return NotFound();

            var skuQr = unitOfWork.Sku
                .GetAll(
                    predicate: o => o.ProductId == product.Id
                );

            var optionGroups = await skuQr.SelectMany(o => o.OptionSkus)
                .GroupBy(optionSku => optionSku.Option.Name)
                .Select(group => new OptionGroup
                {
                    Name = group.Key,
                    Options = group.Select(optionSku => new OptionGroup.Option
                    {
                        Id = optionSku.OptionId,
                        SkuId = optionSku.SkuId,
                        Value = optionSku.Value
                    }).Distinct().ToList()
                })
                .ToListAsync();

            var relatedProducts = await unitOfWork.Product
                .GetAllAsync(
                    predicate: m => m.ProductCategoryId == product.ProductCategoryId && m.Active && m.Id != product.Id,
                    include: m => m.Include(l => l.Skus),
                    selector: m => new Product
                    {
                        Name = m.Name,
                        Url = m.Url,
                        Images = m.Images,
                        Price = m.Price,
                        PriceSale = m.PriceSale,
                        Skus = m.Skus
                    },
                    take: 10,
                    orderBy: m => m.OrderByDescending(o => o.Sort)
                );

            var sku = await GetSkuProduct(product.Id);

            var model = new PageProductViewModel
            {
                Product = product,
                Products = relatedProducts,
                OptionGroups = optionGroups,
            };

            if (sku != null)
            {
                model.MaSP = sku.SKU;
                model.Price = sku.Price;
                model.PriceSale = sku.PriceSale;
                model.SkuId = sku.Id;
            } else
            {
                model.MaSP = product.MaSP;
                model.Price = product.Price;
                model.PriceSale = product.PriceSale;
            }

            return View(model);
        }

        public async Task<Sku?> GetSkuProduct(int id, int? skuId = null)
        {
            var skuQr = unitOfWork.Sku.GetAll(
                    predicate: a => a.ProductId == id,
                    include: m => m.Include(o => o.OptionSkus),
                    orderBy: m => m.OrderBy(o => o.SKU)
                );

            if (skuId.HasValue)
            {
                skuQr = skuQr.Where(a => a.Id == skuId.Value);
            }

            var sku = await skuQr.FirstOrDefaultAsync();

            return sku;
        }

        public async Task<IActionResult> GetSku(int skuId)
        {
            var sku = await unitOfWork.Sku
                .GetAll(
                    predicate: m => m.Id == skuId
                )
                .FirstOrDefaultAsync();

            if (sku == null) return NotFound();

            var finalPrice = sku.PriceSale != decimal.Zero ? sku.PriceSale : sku.Price;

            return Json(new
            {
                sku = sku.SKU ?? "Chưa cập nhật",
                price = finalPrice > 0 ? finalPrice.ToString("N0") + " đ" : "Liên hệ",
                priceOld = sku.Price > 0 ? sku.Price.ToString("N0") + " đ" : null,
            });

        }

        [Route("products")]
        public IActionResult AllProduct(string? category, int? page, string? typeSort, string term = "")
        {
            var list = GetListProduct(page, category, term, typeSort);

            var model = new PageProductViewModel
            {
                ListProduct = list,
                Term = term,
                Sort = typeSort,
                Url = category
            };
            return View(model);
        }

        public IActionResult AllProductView(int? page, string? typeSort, string term = "")
        {
            var list = GetListProduct(page, null, term, typeSort);

            var model = new PageProductViewModel
            {
                ListProduct = list,
                Term = term,
                Sort = typeSort,
            };
            return PartialView(model);
        }

        public IActionResult ListProductView(int? page, string url, string? typeSort, string term = "")
        {
            var list = GetListProduct(page, url, term, typeSort);

            var model = new PageProductViewModel
            {
                ListProduct = list,
                Term = term,
                Url = url,
                Sort = typeSort,
            };
            return PartialView(model);
        }

        [Route("{url:regex(^(?!.*(home|vcms|article|productvcms|upload|banner|contact)).*$)}", Order = 1)]
        public async Task<IActionResult> ListProduct(int? page, string url, string? typeSort, string term = "")
        {
            var category = await unitOfWork.ProductCategory
                .GetAll(
                    predicate: m => m.Url == url && m.Title.Contains(term) && m.Active
                )
                .FirstOrDefaultAsync();

            if (category == null) return NotFound();

            var list = GetListProduct(page, url, term, typeSort);

            var model = new PageProductViewModel
            {
                ProductCategory = category,
                ListProduct = list,
                Term = term,
                Url = url,
                Sort = typeSort,
            };
            return View(model);
        }

        public IPagedList<Product> GetListProduct(int? page, string? url, string term = "", string typeSort = "", bool isPromotion = false)
        {
            int pageNumber = page ?? 1;

            var productsQuery = unitOfWork.Product
                .GetAll(
                    predicate: m => m.Name.Contains(term) && m.Active,
                    include: m => m
                    .Include(l => l.ProductCategory)
                    .ThenInclude(l => l.ParentCategory)
                    .Include(l => l.Skus)
                );

            if (!string.IsNullOrEmpty(url))
            {
                var category = unitOfWork.ProductCategory.GetAll(
                        predicate: m => m.Url == url
                    ).FirstOrDefault();

                if (category != null)
                {
                    productsQuery = productsQuery.Where(m =>
                        m.ProductCategoryId == category.Id ||
                        m.ProductCategory.ParentCategoryId == category.Id ||
                        m.ProductCategory.ParentCategory.ParentCategoryId == category.Id);
                }
            }

            productsQuery = typeSort switch
            {
                "name-desc" => productsQuery.OrderBy(p => p.Name),
                "name-asc" => productsQuery.OrderByDescending(p => p.Name),
                "date-desc" => productsQuery.OrderBy(p => p.CreatedAt),
                "date-asc" => productsQuery.OrderByDescending(p => p.CreatedAt),
                _ => productsQuery.OrderByDescending(p => p.Sort)
            };

            var filteredProducts = productsQuery.AsEnumerable();

            if (isPromotion)
            {
                filteredProducts = filteredProducts.Where(p => p.Percent > 0);
            }

            return filteredProducts.ToPagedList(pageNumber, 12);
        }

        [Route("blog/{url}.html", Order = 0)]
        public async Task<IActionResult> ArticleDetails(string url)
        {
            var article = await unitOfWork.Article
                .GetAll(
                    predicate: m => m.Url == url,
                    include: m => m.Include(l => l.ArticleCategory),
                    disableTracking: false
                ).FirstOrDefaultAsync();

            if (article == null) return NotFound();

            article.Views += 1;
            await unitOfWork.CommitAsync();

            var articles = await unitOfWork.Article.GetAllAsync(
                    predicate: a => a.ArticleCategoryId == article.ArticleCategoryId && a.Id != article.Id,
                    include: a => a.Include(l => l.ArticleCategory),
                    orderBy: a => a.OrderByDescending(a => a.Sort),
                    take: 4
                );

            var model = new PageArticleViewModel
            {
                Article = article,
                Articles = articles,
            };
            return View(model);
        }

        [Route("blog")]
        public async Task<IActionResult> AllArticle(int? page, string term = "")
        {
            var pageNumber = page ?? 1;

            var listArticle = await unitOfWork.Article
                .GetPagedListAsync(
                    predicate: m => m.Name.Contains(term) && m.Active,
                    orderBy: a => a.OrderByDescending(c => c.Sort),
                    include: a => a.Include(l => l.ArticleCategory),
                    pageIndex: pageNumber,
                    pageSize: 9
                );

            var model = new PageArticleViewModel
            {
                ListArticle = listArticle,
            };
            return View(model);
        }

        [Route("blog/{url:regex(^(?!.*(home|vcms|article|productvcms|upload|banner|contact)).*$)}", Order = 1)]
        public async Task<IActionResult> ListArticle(int? page, string url, string term = "")
        {
            int pageNumber = page ?? 1;

            var category = await unitOfWork.ArticleCategory
                .GetAll(
                    predicate: m => m.Url == url && m.Title.Contains(term) && m.Active
                ).FirstOrDefaultAsync();

            if (category == null) return NotFound();

            var listArticle = await unitOfWork.Article
                .GetPagedListAsync(
                    predicate: m => m.Active &&  (m.ArticleCategoryId == category.Id || m.ArticleCategory.ParentCategoryId == category.Id),
                    orderBy: a => a.OrderByDescending(c => c.Sort),
                    include: a => a.Include(l => l.ArticleCategory),
                    pageIndex: pageNumber,
                    pageSize: 9
                );

            var model = new PageArticleViewModel
            {
                ArticleCategory = category,
                ListArticle = listArticle
            };
            return View(model);
        }

        [Route("lien-he")]
        public IActionResult Contact() => View();

        [HttpPost]
        public async Task<IActionResult> ContactStore(Contact contact)
        {
            if (ModelState.IsValid)
            {
                await unitOfWork.Contact.InsertAsync(contact);
                await unitOfWork.CommitAsync();

                var subjectMail = "Email liên hệ từ website: " + Request.Host;
                var bodyMail = $"<p>Tên người liên hệ: {contact.FullName},</p>" +
                               $"<p>Số điện thoại: {contact.PhoneNumber},</p>" +
                               $"<p>Email: {contact.Email},</p>" +
                               $"<p>Nội dung: {contact.Body}</p>";

                var mailData = new MailData
                {
                    EmailToId = appService.Config.Email,
                    EmailSubject = subjectMail,
                    EmailBody = bodyMail
                };

                await mailService.SendEmailAsync(mailData);
                return Json(new { status = true, msg = "Gửi liên hệ thành công.\nChúng tôi sẽ liên lạc với bạn sớm nhất có thể." });
            }
            return Json(new { status = false, msg = "Quá trình thực hiện không thành công." });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Xml.Linq;
using Evarosa.Data;

namespace Evarosa.Controllers
{
    public class SitemapController(ApplicationDbContext context) : Controller
    {
        [Route("sitemap.xml")]
        public ActionResult Index()
        {
            Request.Headers.Add("Content-Type", "text/xml");
            return PartialView();
        }

        #region Sitemap - Product
        [OutputCache(Duration = 84600)]
        [Route("sitemap/products.xml")]
        public ContentResult ProductSitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var items = context.Products.Where(a => a.Active).OrderByDescending(a => a.Id).Select(a => new { a.Url, a.CreatedAt }).ToList();
            var itemSitemap = (from item in items
                               select new XElement(ns + "url", new XElement(ns + "loc", Url.Action("ProductDetails", "Home", new
                               {
                                   url = item.Url
                               }, protocol: Request.Scheme)), new XElement(ns + "lastmod", item.CreatedAt.ToString("yyyy-MM-dd")), new XElement(ns + "changefreq", "daily"), new XElement(ns + "priority", "0.8"))).ToList();
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(ns + "urlset", itemSitemap));
            return Content(sitemap.ToString(), "text/xml");
            //sitemap.Save(Server.MapPath("/Sitemap/ArticleSitemap.xml"));
        }
        #endregion

        #region Sitemap - ProductCategory
        [Route("sitemap/product-categories.xml")]
        public ContentResult ProductCategorySitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var items = context.ProductCategories.Where(a => a.Active).OrderBy(a => a.Id).Select(a => new { a.Url }).ToList();
            var itemSitemap = (from item in items
                select new XElement(ns + "url", new XElement(ns + "loc", Url.Action("ListProduct", "Home", new
                {
                    url = item.Url
                }, protocol: Request.Scheme)), new XElement(ns + "lastmod", DateTime.Now.ToString("yyyy-MM-dd")), new XElement(ns + "changefreq", "daily"), new XElement(ns + "priority", "0.8"))).ToList();
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(ns + "urlset", itemSitemap));
            return Content(sitemap.ToString(), "text/xml");
            //sitemap.Save(Server.MapPath("/Sitemap/ArticleCategorySitemap.xml"));
        }
        #endregion

        #region Sitemap - Article
        [OutputCache(Duration = 84600)]
        [Route("sitemap/articles.xml")]
        public ContentResult ArticleSitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var items = context.Articles.Where(a => a.Active).OrderByDescending(a => a.CreatedAt).Select(a => new { a.Url, a.CreatedAt }).ToList();
            var itemSitemap = (from item in items
                               select new XElement(ns + "url", new XElement(ns + "loc", Url.Action("ArticleDetails", "Home", new
                               {
                                   url = item.Url
                               }, protocol: Request.Scheme)), new XElement(ns + "lastmod", item.CreatedAt.ToString("yyyy-MM-dd")), new XElement(ns + "changefreq", "daily"), new XElement(ns + "priority", "0.8"))).ToList();
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(ns + "urlset", itemSitemap));
            return Content(sitemap.ToString(), "text/xml");
            //sitemap.Save(Server.MapPath("/Sitemap/ArticleSitemap.xml"));
        }
        #endregion

        #region Sitemap - ArticleCategory
        [Route("sitemap/categories.xml")]
        public ContentResult ArticleCategorySitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var items = context.ArticleCategories.Where(a => a.Active).OrderBy(a => a.Id).Select(a => new { a.Url }).ToList();
            var itemSitemap = (from item in items
                               select new XElement(ns + "url", new XElement(ns + "loc", Url.Action("ListArticle", "Home", new
                               {
                                   url = item.Url
                               }, protocol: Request.Scheme)), new XElement(ns + "lastmod", DateTime.Now.ToString("yyyy-MM-dd")), new XElement(ns + "changefreq", "daily"), new XElement(ns + "priority", "0.8"))).ToList();
            var sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(ns + "urlset", itemSitemap));
            return Content(sitemap.ToString(), "text/xml");
            //sitemap.Save(Server.MapPath("/Sitemap/ArticleCategorySitemap.xml"));
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
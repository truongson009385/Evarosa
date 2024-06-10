using Evarosa.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace Evarosa.ViewModels
{
    public class BannerViewModel
    {
        public string ItemsFineJSON { get; set; }
        public Banner Banner { get; set; }
        public SelectList SelectGroup { get; set; }
        public IFormFile? Image { get; set; }
        public BannerViewModel()
        {
            var listgroup = new Dictionary<int, string>
            {
                { 1, "Banner" },
                { 2, "Tại sao?" },
                { 3, "Nhân viên" },
                { 4, "Thanh bên" },
            };
            SelectGroup = new SelectList(listgroup, "Key", "Value");
        }
    }

    public class ListBannerViewModel
    {
        public IPagedList<Banner> Banners { get; set; }
        public int? GroupId { get; set; }
        public SelectList SelectGroup { get; set; }
        public ListBannerViewModel()
        {
            var listgroup = new Dictionary<int, string>
            {
                { 1, "Banner" },
                { 2, "Tại sao?" },
                { 3, "Nhân viên" },
                { 4, "Thanh bên" },
            };
            SelectGroup = new SelectList(listgroup, "Key", "Value");
        }
    }
}

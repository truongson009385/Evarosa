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
        public string Image { get; set; }
        public BannerViewModel()
        {
            var listgroup = new Dictionary<int, string>
            {
                { 1, "Quảng cáo" },
                { 2, "Đối tác - Khách hàng" },
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
                { 1, "Quảng cáo" },
                { 2, "Đối tác - Khách hàng" },
            };
            SelectGroup = new SelectList(listgroup, "Key", "Value");
        }
    }
}

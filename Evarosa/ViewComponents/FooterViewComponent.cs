using Microsoft.AspNetCore.Mvc;

namespace GoVietTrip.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int? id)
        {
            return View();
        }
    }
}

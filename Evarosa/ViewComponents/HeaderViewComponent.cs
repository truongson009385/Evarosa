using Microsoft.AspNetCore.Mvc;

namespace Evarosa.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace Evarosa.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int? id)
        {
            return View();
        }
    }
}

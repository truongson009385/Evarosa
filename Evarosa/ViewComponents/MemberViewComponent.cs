using Microsoft.AspNetCore.Mvc;

namespace Evarosa.ViewComponents
{
    public class MemberViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

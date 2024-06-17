using Evarosa.Data;
using Evarosa.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Evarosa.ViewComponents
{
    public class MemberViewComponent(UnitOfWork unitOfWork) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var emailClaim = UserClaimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var member = unitOfWork.Member.GetAll(
                    predicate: m => m.Email == emailClaim
                ).FirstOrDefault();

            var model = new MemberComponentViewModel
            {
                Member = member
            };
            return View(model);
        }
    }
}

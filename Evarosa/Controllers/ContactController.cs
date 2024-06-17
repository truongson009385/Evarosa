using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Evarosa.Data;
using Evarosa.Services;
using Evarosa.Services.Impl;
using Evarosa.ViewModels;
using System.Drawing;
using X.PagedList;

namespace Evarosa.Controllers
{
    [Authorize(AuthenticationSchemes = "vcms", Roles = "Admin,Editor")]
    public class ContactController(UnitOfWork unitOfWork, IFileService fileService) : Controller
    {
        #region Contact
        public IActionResult ListContact(int? page, string? name)
        {
            var pageNumber = page ?? 1;
            var contact = unitOfWork.Contact.GetAll();

            if (!string.IsNullOrEmpty(name))
            {
                contact = contact.Where(m => m.FullName.Contains(name));
            }
            var model = new ContactViewModel
            {
                ListContact = contact.ToPagedList(pageNumber, 20),
                Name = name
            };
            return View(model);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<bool> DeleteContact(int contactId = 0)
        {
            var contact = await unitOfWork.Contact.FindAsync(contactId);
            if (contact == null)
            {
                return false;
            }
            unitOfWork.Contact.Delete(contact);
            unitOfWork.Commit();
            return true;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Rollback();
            base.Dispose(disposing);
        }
    }
}

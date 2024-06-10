using Evarosa.Models;
using X.PagedList;

namespace Evarosa.ViewModels
{
    public class ContactViewModel
    {
        public string? Name { get; set; }
        public IPagedList<Contact> ListContact { get; set; }
    }
}

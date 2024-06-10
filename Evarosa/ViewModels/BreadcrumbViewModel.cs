namespace Evarosa.ViewModels
{
    public class BreadcrumbViewModel
    {
        public string Title { get; set; }
        public string? Image { get; set; }
        public List<BreadcrumbItem> Path { get; set; } = new List<BreadcrumbItem>();
    }

    public class BreadcrumbItem
    {
        public string Key { get; set; }
        public string? Value { get; set; }
    }
}

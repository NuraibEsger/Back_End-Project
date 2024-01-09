using BackEndProject.Entities;

namespace BackEndProject.Models
{
    public class HomeIndexVM
    {
        public List<Product> Products { get; set; }
        public string? SortOrder { get; set; }
    }
}

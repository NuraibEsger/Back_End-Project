using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace BackEndProject.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}

namespace BackEndProject.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int ColorId { get; set; }
        public Brand? Brand { get; set; }
        public Color? Color { get; set; }
        public Category? Category { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
    }
}

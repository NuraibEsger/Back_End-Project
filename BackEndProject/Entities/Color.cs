namespace BackEndProject.Entities
{
    public class Color
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public List<Product>? Products { get; set; }
    }
}

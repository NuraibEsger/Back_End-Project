using BackEndProject.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BackEndProject.Areas.Admin.Models
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(55)]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        [Range(0, double.PositiveInfinity)]
        public decimal Price { get; set; }
        public List<IFormFile>? Photo { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required]
        public int ColorId { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public List<SelectListItem>? Colors { get; set; }
        public List<SelectListItem>? Brands { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
    }
}

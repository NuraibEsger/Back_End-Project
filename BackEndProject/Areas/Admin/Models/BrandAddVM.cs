using System.ComponentModel.DataAnnotations;

namespace BackEndProject.Areas.Admin.Models
{
    public class BrandAddVM
    {
        [Required]
        [MinLength(2)]
        [MaxLength(55)]
        public string? Name { get; set; }
    }
}

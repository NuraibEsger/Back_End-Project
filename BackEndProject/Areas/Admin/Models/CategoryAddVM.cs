using System.ComponentModel.DataAnnotations;

namespace BackEndProject.Areas.Admin.Models
{
    public class CategoryAddVM
    {
        [Required]
        [MinLength(2)]
        [MaxLength(55)]
        public string? CategoryName { get; set; }
    }
}

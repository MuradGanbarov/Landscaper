using System.ComponentModel.DataAnnotations;

namespace Landscaper.Areas.Admin.ViewModels
{
    public class ServiceCreateVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "Name can contain minimum 3 characters")]
        [MaxLength(25, ErrorMessage = "Name can contain maximum 25 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MinLength(5, ErrorMessage = "Description can contain minimum 5 characters")]
        [MaxLength(300, ErrorMessage = "Description can contain maximum 300 characters")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Photo is required")]
        public IFormFile Photo { get; set; }
    }
}

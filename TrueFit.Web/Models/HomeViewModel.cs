using System.ComponentModel.DataAnnotations;

namespace TrueFit.Web.Models
{
    public class HomeViewModel
    {
        [Required]
        [Display(Name="Upload File")]
        public IFormFile UploadFile { get; set; } = null!;
    }
}

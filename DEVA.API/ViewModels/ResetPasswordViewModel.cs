using System.ComponentModel.DataAnnotations;

namespace DEVA.API.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}
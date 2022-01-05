using System.ComponentModel.DataAnnotations;

namespace DEVA.API.Controllers
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
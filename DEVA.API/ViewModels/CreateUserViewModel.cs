using System.ComponentModel.DataAnnotations;

namespace DEVA.API.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
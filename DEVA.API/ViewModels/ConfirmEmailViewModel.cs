using System.ComponentModel.DataAnnotations;

namespace DEVA.API.ViewModels
{
    public class ConfirmEmailViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
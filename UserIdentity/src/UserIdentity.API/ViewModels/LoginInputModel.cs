using System.ComponentModel.DataAnnotations;

namespace UserIdentity.API.ViewModels
{
    public class LoginInputModel
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
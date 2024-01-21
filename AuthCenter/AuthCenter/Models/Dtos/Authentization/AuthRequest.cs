using System.ComponentModel.DataAnnotations;

namespace AuthCenter.Models.Dtos.Authentization
{
    public class AuthRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

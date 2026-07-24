using System.ComponentModel.DataAnnotations;

namespace QuickCommerce.Core.DTOs.Admin
{
    public class ForgotPasswordRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Auth
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(64)]
        [RegularExpression("^[a-zA-Z0-9._-]{3,64}$", ErrorMessage = "Username ch? g?m ch?, s?, d?u ch?m, g?ch du?i ho?c g?ch ngang.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(128)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "M?t kh?u nh?p l?i không kh?p.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool AcceptTerms { get; set; }
    }
}

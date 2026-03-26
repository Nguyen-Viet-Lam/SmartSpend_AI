using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Auth
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "OTP ph?i g?m 6 ch? s?.")]
        public string OtpCode { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "M?t kh?u nh?p l?i không kh?p.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

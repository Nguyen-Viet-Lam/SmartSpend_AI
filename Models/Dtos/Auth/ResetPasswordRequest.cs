using System.ComponentModel.DataAnnotations;

namespace Web_Project.Models
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "OTP phai gom 6 chu so.")]
        public string OtpCode { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Mat khau nhap lai khong khop.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

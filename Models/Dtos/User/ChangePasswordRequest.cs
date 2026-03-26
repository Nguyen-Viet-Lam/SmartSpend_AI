using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.User
{
    public class ChangePasswordRequest
    {
        [Required]
        [MaxLength(128)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu nhập lại không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

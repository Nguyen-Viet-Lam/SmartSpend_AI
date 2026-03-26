using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Auth
{
    public class VerifyEmailOtpRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "OTP ph?i g?m 6 ch? s?.")]
        public string OtpCode { get; set; } = string.Empty;
    }
}

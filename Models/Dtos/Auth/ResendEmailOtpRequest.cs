using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Auth
{
    public class ResendEmailOtpRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;
    }
}

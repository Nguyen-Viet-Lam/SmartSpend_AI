using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.User
{
    public class UpdateProfileRequest
    {
        [Required]
        [MaxLength(128)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Url]
        [MaxLength(500)]
        public string? AvatarUrl { get; set; }
    }
}

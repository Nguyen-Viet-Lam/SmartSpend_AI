using System.ComponentModel.DataAnnotations;

namespace Web_Project.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        public int? ActorUserId { get; set; }

        [Required]
        [MaxLength(120)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string TargetType { get; set; } = string.Empty;

        [MaxLength(80)]
        public string TargetId { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Metadata { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public User? ActorUser { get; set; }
    }
}

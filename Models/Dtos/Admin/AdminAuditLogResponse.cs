namespace SmartSpendAI.Models.Dtos.Admin
{
    public class AdminAuditLogResponse
    {
        public int AuditLogId { get; set; }

        public string Actor { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string TargetType { get; set; } = string.Empty;

        public string TargetId { get; set; } = string.Empty;

        public string Metadata { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}

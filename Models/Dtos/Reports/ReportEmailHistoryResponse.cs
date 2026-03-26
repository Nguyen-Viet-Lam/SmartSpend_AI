namespace SmartSpendAI.Models.Dtos.Reports
{
    public class ReportEmailHistoryResponse
    {
        public int AuditLogId { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string RecipientEmail { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Metadata { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }
    }
}

namespace SmartSpendAI.Models
{
    public class WeeklySummaryEmailSettings
    {
        public bool Enabled { get; set; } = false;

        public string TimeZoneId { get; set; } = "Asia/Bangkok";

        public int SendHourLocal { get; set; } = 8;

        public int SendMinuteLocal { get; set; } = 0;

        public int CheckIntervalMinutes { get; set; } = 10;
    }
}

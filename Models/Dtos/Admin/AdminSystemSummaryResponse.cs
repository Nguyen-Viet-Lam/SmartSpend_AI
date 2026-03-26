namespace SmartSpendAI.Models.Dtos.Admin
{
    public class AdminSystemSummaryResponse
    {
        public int NewUsersToday { get; set; }

        public int TransactionsToday { get; set; }

        public int TotalUsers { get; set; }

        public int TotalKeywords { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Web_Project.Models.Dtos.Finance
{
    public class BudgetRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public DateTime Month { get; set; }

        [Range(1, 999999999)]
        public decimal LimitAmount { get; set; }
    }

    public class BudgetResponse
    {
        public int BudgetId { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string CategoryColor { get; set; } = string.Empty;

        public DateTime Month { get; set; }

        public decimal LimitAmount { get; set; }

        public decimal SpentAmount { get; set; }

        public decimal ProgressPercentage { get; set; }

        public string Status { get; set; } = "Safe";
    }
}

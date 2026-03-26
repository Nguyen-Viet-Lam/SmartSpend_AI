using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Finance
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
}

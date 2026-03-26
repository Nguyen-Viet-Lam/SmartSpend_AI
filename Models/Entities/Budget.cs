using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }

        public DateTime Month { get; set; }

        public decimal LimitAmount { get; set; }

        public User User { get; set; } = null!;

        public Category Category { get; set; } = null!;
    }
}

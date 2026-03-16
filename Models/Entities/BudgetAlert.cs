using System.ComponentModel.DataAnnotations;

namespace Web_Project.Models
{
    public class BudgetAlert
    {
        [Key]
        public int BudgetAlertId { get; set; }

        public int UserId { get; set; }

        public int TransactionId { get; set; }

        [Required]
        [MaxLength(280)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        public string Level { get; set; } = "Warning";

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;

        public TransactionEntry Transaction { get; set; } = null!;
    }
}

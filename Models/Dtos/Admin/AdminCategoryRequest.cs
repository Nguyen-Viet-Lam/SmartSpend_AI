using System.ComponentModel.DataAnnotations;

namespace SmartSpendAI.Models.Dtos.Admin
{
    public class AdminCategoryRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        public string Type { get; set; } = "Expense";

        [MaxLength(48)]
        public string? Icon { get; set; }

        [MaxLength(16)]
        public string? Color { get; set; }

        public bool IsSystem { get; set; }
    }
}

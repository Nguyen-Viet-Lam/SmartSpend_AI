namespace SmartSpendAI.Models.Dtos.Admin
{
    public class AdminCategoryResponse
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public bool IsSystem { get; set; }
    }
}

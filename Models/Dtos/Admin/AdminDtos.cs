using System.ComponentModel.DataAnnotations;

namespace Web_Project.Models.Dtos.Admin
{
    public class AdminUserSummaryResponse
    {
        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsLocked { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class KeywordRequest
    {
        [Required]
        [MaxLength(100)]
        public string Word { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Range(1, 20)]
        public int Weight { get; set; } = 5;

        public bool IsActive { get; set; } = true;
    }

    public class KeywordResponse
    {
        public int KeywordId { get; set; }

        public string Word { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int Weight { get; set; }

        public bool IsActive { get; set; }
    }

    public class AdminSystemSummaryResponse
    {
        public int NewUsersToday { get; set; }

        public int TransactionsToday { get; set; }

        public int TotalUsers { get; set; }

        public int TotalKeywords { get; set; }
    }
}

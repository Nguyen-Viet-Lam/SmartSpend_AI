using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Web_Project.Models;
using Web_Project.Models.Dtos.Finance;

namespace Web_Project.Services.AI
{
    public class SmartInputService : ISmartInputService
    {
        private readonly AppDbContext _dbContext;

        public SmartInputService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SmartInputResponse> ParseAsync(string input, CancellationToken cancellationToken)
        {
            var normalized = Normalize(input);
            var amount = ExtractAmount(normalized);
            var transactionDate = ExtractDate(normalized);

            var keywords = await _dbContext.Keywords
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);

            var matchedKeywords = new List<string>();
            Category? category = null;
            var bestScore = 0;

            foreach (var keyword in keywords)
            {
                var normalizedKeyword = Normalize(keyword.Word);
                if (!normalized.Contains(normalizedKeyword, StringComparison.Ordinal))
                {
                    continue;
                }

                matchedKeywords.Add(keyword.Word);
                if (keyword.Weight > bestScore)
                {
                    bestScore = keyword.Weight;
                    category = keyword.Category;
                }
            }

            var confidence = 0.25m;
            if (amount > 0)
            {
                confidence += 0.35m;
            }

            if (category is not null)
            {
                confidence += 0.25m;
            }

            if (!transactionDate.Date.Equals(DateTime.UtcNow.Date))
            {
                confidence += 0.10m;
            }

            confidence += Math.Min(0.15m, matchedKeywords.Count * 0.03m);

            return new SmartInputResponse
            {
                Amount = amount,
                SuggestedCategoryId = category?.CategoryId,
                SuggestedCategoryName = category?.Name ?? string.Empty,
                TransactionDate = transactionDate,
                NormalizedNote = BuildNormalizedNote(input),
                AiConfidence = Math.Min(0.98m, confidence),
                MatchedKeywords = matchedKeywords
            };
        }

        private static string BuildNormalizedNote(string input)
        {
            return Regex.Replace(input.Trim(), "\\s+", " ");
        }

        private static decimal ExtractAmount(string normalized)
        {
            var millionMatch = Regex.Match(normalized, @"(?<!\d)(\d+)\s*tr(\d{1,3})?(?!\d)");
            if (millionMatch.Success)
            {
                var leading = decimal.Parse(millionMatch.Groups[1].Value, CultureInfo.InvariantCulture) * 1_000_000m;
                var suffix = millionMatch.Groups[2].Value;
                if (!string.IsNullOrWhiteSpace(suffix))
                {
                    var suffixValue = decimal.Parse(suffix, CultureInfo.InvariantCulture);
                    var multiplier = (decimal)Math.Pow(10, 6 - suffix.Length);
                    leading += suffixValue * multiplier;
                }

                return leading;
            }

            var kiloMatch = Regex.Match(normalized, @"(?<!\d)(\d+(?:[.,]\d+)?)\s*k(?!\w)");
            if (kiloMatch.Success)
            {
                var raw = kiloMatch.Groups[1].Value.Replace(",", ".");
                return decimal.Parse(raw, CultureInfo.InvariantCulture) * 1_000m;
            }

            var separatorMatch = Regex.Match(normalized, @"(?<!\d)(\d{1,3}(?:[.,]\d{3})+)(?!\d)");
            if (separatorMatch.Success)
            {
                var cleaned = separatorMatch.Groups[1].Value.Replace(".", string.Empty).Replace(",", string.Empty);
                return decimal.Parse(cleaned, CultureInfo.InvariantCulture);
            }

            var plainMatch = Regex.Match(normalized, @"(?<!\d)(\d{4,})(?!\d)");
            if (plainMatch.Success)
            {
                return decimal.Parse(plainMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            }

            return 0m;
        }

        private static DateTime ExtractDate(string normalized)
        {
            var today = DateTime.UtcNow.Date;

            if (normalized.Contains("hom qua", StringComparison.Ordinal))
            {
                return today.AddDays(-1);
            }

            if (normalized.Contains("tuan truoc", StringComparison.Ordinal))
            {
                return today.AddDays(-7);
            }

            if (normalized.Contains("hom nay", StringComparison.Ordinal) ||
                normalized.Contains("sang nay", StringComparison.Ordinal) ||
                normalized.Contains("chieu nay", StringComparison.Ordinal) ||
                normalized.Contains("toi nay", StringComparison.Ordinal))
            {
                return today;
            }

            var explicitDate = Regex.Match(normalized, @"(?<!\d)(\d{1,2})[/-](\d{1,2})(?:[/-](\d{2,4}))?(?!\d)");
            if (explicitDate.Success)
            {
                var day = int.Parse(explicitDate.Groups[1].Value, CultureInfo.InvariantCulture);
                var month = int.Parse(explicitDate.Groups[2].Value, CultureInfo.InvariantCulture);
                var year = explicitDate.Groups[3].Success
                    ? int.Parse(explicitDate.Groups[3].Value, CultureInfo.InvariantCulture)
                    : today.Year;

                if (year < 100)
                {
                    year += 2000;
                }

                try
                {
                    return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
                }
                catch
                {
                    return today;
                }
            }

            return today;
        }

        private static string Normalize(string input)
        {
            var text = RemoveDiacritics(input ?? string.Empty).ToLowerInvariant();
            text = Regex.Replace(text, @"[^a-z0-9/\-\s\.,]", " ");
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var ch in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(ch);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC)
                .Replace('đ', 'd')
                .Replace('Đ', 'D');
        }
    }
}

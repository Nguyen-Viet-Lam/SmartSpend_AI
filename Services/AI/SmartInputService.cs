using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SmartSpendAI.Models;
using SmartSpendAI.Models.Dtos.Finance;

namespace SmartSpendAI.Services.AI
{
    public class SmartInputService : ISmartInputService
    {
        private readonly AppDbContext _dbContext;
        private static readonly HashSet<string> NoiseKeywords =
        [
            "chi",
            "thu",
            "mua",
            "tra",
            "cho",
            "va",
            "tu",
            "den",
            "hom",
            "nay",
            "qua",
            "tuan",
            "thang",
            "nam"
        ];

        public SmartInputService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SmartInputResponse> ParseAsync(string input, int userId, CancellationToken cancellationToken)
        {
            var normalized = Normalize(input);
            var amount = ExtractAmount(normalized);
            var transactionDate = ExtractDate(normalized);

            var matchedKeywords = new List<string>();
            Category? category = null;
            var usedPersonalKeyword = false;

            var personalKeywords = await _dbContext.UserPersonalKeywords
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.UsageCount)
                .ThenByDescending(x => x.Keyword.Length)
                .ToListAsync(cancellationToken);

            foreach (var personalKeyword in personalKeywords)
            {
                var normalizedKeyword = Normalize(personalKeyword.Keyword);
                if (string.IsNullOrWhiteSpace(normalizedKeyword))
                {
                    continue;
                }

                if (!normalized.Contains(normalizedKeyword, StringComparison.Ordinal))
                {
                    continue;
                }

                matchedKeywords.Add(personalKeyword.Keyword);
                category = personalKeyword.Category;
                usedPersonalKeyword = true;
                break;
            }

            if (!usedPersonalKeyword)
            {
                var keywords = await _dbContext.Keywords
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Where(x => x.IsActive)
                    .ToListAsync(cancellationToken);

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
            }

            var confidence = 0.20m;
            if (amount > 0)
            {
                confidence += 0.30m;
            }

            if (category is not null)
            {
                confidence += usedPersonalKeyword ? 0.40m : 0.25m;
            }

            if (!transactionDate.Date.Equals(DateTime.UtcNow.Date))
            {
                confidence += 0.08m;
            }

            confidence += Math.Min(0.18m, matchedKeywords.Count * 0.03m);

            return new SmartInputResponse
            {
                Amount = amount,
                SuggestedCategoryId = category?.CategoryId,
                SuggestedCategoryName = category?.Name ?? string.Empty,
                TransactionDate = transactionDate,
                NormalizedNote = BuildNormalizedNote(input),
                AiConfidence = Math.Min(0.99m, confidence),
                MatchedKeywords = matchedKeywords
            };
        }

        public async Task LearnFromCorrectionAsync(string input, int userId, int correctedCategoryId, CancellationToken cancellationToken)
        {
            var categoryExists = await _dbContext.Categories
                .AsNoTracking()
                .AnyAsync(x => x.CategoryId == correctedCategoryId, cancellationToken);

            if (!categoryExists)
            {
                throw new InvalidOperationException("Danh muc khong ton tai.");
            }

            var normalizedInput = Normalize(input);
            if (string.IsNullOrWhiteSpace(normalizedInput))
            {
                return;
            }

            var learningKeywords = ExtractLearningKeywords(normalizedInput);
            if (learningKeywords.Count == 0)
            {
                return;
            }

            var now = DateTime.UtcNow;
            foreach (var keyword in learningKeywords)
            {
                var existing = await _dbContext.UserPersonalKeywords
                    .FirstOrDefaultAsync(
                        x => x.UserId == userId && x.Keyword == keyword,
                        cancellationToken);

                if (existing is null)
                {
                    _dbContext.UserPersonalKeywords.Add(new UserPersonalKeyword
                    {
                        UserId = userId,
                        CategoryId = correctedCategoryId,
                        Keyword = keyword,
                        UsageCount = 1,
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
                else
                {
                    existing.CategoryId = correctedCategoryId;
                    existing.UsageCount += 1;
                    existing.UpdatedAt = now;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private static List<string> ExtractLearningKeywords(string normalizedInput)
        {
            var keywords = new HashSet<string>(StringComparer.Ordinal);
            var compact = Regex.Replace(normalizedInput, "\\s+", " ").Trim();
            if (compact.Length >= 3)
            {
                keywords.Add(compact);
            }

            foreach (var token in compact.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (token.Length < 3 || NoiseKeywords.Contains(token))
                {
                    continue;
                }

                if (decimal.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    continue;
                }

                keywords.Add(token);
            }

            return keywords.Take(8).ToList();
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
                .Replace('\u0111', 'd')
                .Replace('\u0110', 'D');
        }
    }
}

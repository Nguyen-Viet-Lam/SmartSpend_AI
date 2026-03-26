using Microsoft.EntityFrameworkCore;
using SmartSpendAI.Models;
using SmartSpendAI.Services.AI;

namespace SmartSpendAI.Tests.AI;

public sealed class SmartInputServiceTests
{
    [Fact]
    public async Task ParseAsync_PrioritizesPersonalKeywordOverSystemKeyword()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Categories.AddRange(
            new Category { CategoryId = 1, Name = "An uong", Type = "Expense", Icon = "utensils", Color = "#ff7a18", IsSystem = true },
            new Category { CategoryId = 2, Name = "Di chuyen", Type = "Expense", Icon = "car", Color = "#00b894", IsSystem = true });
        dbContext.Keywords.Add(new KeywordEntry
        {
            KeywordEntryId = 1,
            Word = "xang",
            CategoryId = 2,
            Weight = 10,
            IsActive = true
        });
        dbContext.UserPersonalKeywords.Add(new UserPersonalKeyword
        {
            UserPersonalKeywordId = 1,
            UserId = 7,
            CategoryId = 1,
            Keyword = "xang",
            UsageCount = 3
        });
        await dbContext.SaveChangesAsync();

        var service = new SmartInputService(dbContext);
        var result = await service.ParseAsync("Do xang 200k", 7, CancellationToken.None);

        Assert.Equal(1, result.SuggestedCategoryId);
        Assert.Contains("xang", result.MatchedKeywords);
    }

    [Fact]
    public async Task LearnFromCorrectionAsync_UpsertsPersonalKeyword()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Categories.AddRange(
            new Category { CategoryId = 10, Name = "Khac", Type = "Expense", Icon = "circle", Color = "#94a3b8", IsSystem = true },
            new Category { CategoryId = 11, Name = "Hoc tap", Type = "Expense", Icon = "book", Color = "#2563eb", IsSystem = true });
        await dbContext.SaveChangesAsync();

        var service = new SmartInputService(dbContext);
        await service.LearnFromCorrectionAsync("Mua sach 150k", 22, 10, CancellationToken.None);
        await service.LearnFromCorrectionAsync("Mua sach 150k", 22, 11, CancellationToken.None);

        var learned = await dbContext.UserPersonalKeywords
            .SingleAsync(x => x.UserId == 22 && x.Keyword == "mua sach 150k");

        Assert.Equal(11, learned.CategoryId);
        Assert.Equal(2, learned.UsageCount);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"smart-input-tests-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }
}

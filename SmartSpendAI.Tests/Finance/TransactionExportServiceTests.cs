using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SmartSpendAI.Models;
using SmartSpendAI.Models.Dtos.Finance;
using SmartSpendAI.Services.Finance;

namespace SmartSpendAI.Tests.Finance;

public sealed class TransactionExportServiceTests
{
    [Fact]
    public async Task ExportAsync_ReturnsWorkbookFilteredByQuery()
    {
        await using var dbContext = CreateDbContext();
        SeedTransactions(dbContext);

        var service = new TransactionExportService(dbContext);
        var payload = await service.ExportAsync(
            100,
            new TransactionExportFilter
            {
                From = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2026, 3, 31, 0, 0, 0, DateTimeKind.Utc),
                Type = "Expense"
            },
            CancellationToken.None);

        using var stream = new MemoryStream(payload);
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet("Transactions");

        Assert.Equal("TransactionDate", sheet.Cell(1, 1).GetString());
        Assert.Equal("An trua", sheet.Cell(2, 2).GetString());
        Assert.Equal("Expense", sheet.Cell(2, 5).GetString());
        Assert.Equal(1, sheet.LastRowUsed()?.RowNumber() - 1);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"transaction-export-tests-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }

    private static void SeedTransactions(AppDbContext dbContext)
    {
        dbContext.Roles.Add(new Role { RoleId = 1, RoleName = "StandardUser" });
        dbContext.Users.AddRange(
            new User
            {
                UserId = 100,
                Username = "demo100",
                FullName = "Demo 100",
                Email = "demo100@local",
                PasswordHash = "hash",
                RoleId = 1,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                UserId = 200,
                Username = "demo200",
                FullName = "Demo 200",
                Email = "demo200@local",
                PasswordHash = "hash",
                RoleId = 1,
                CreatedAt = DateTime.UtcNow
            });
        dbContext.Wallets.AddRange(
            new Wallet
            {
                WalletId = 1,
                UserId = 100,
                Name = "Tien mat",
                Type = "Cash",
                Balance = 1_000_000m,
                IsDefault = true,
                CreatedAt = DateTime.UtcNow
            },
            new Wallet
            {
                WalletId = 2,
                UserId = 200,
                Name = "Tien mat 200",
                Type = "Cash",
                Balance = 500_000m,
                IsDefault = true,
                CreatedAt = DateTime.UtcNow
            });
        dbContext.Categories.AddRange(
            new Category { CategoryId = 1, Name = "An uong", Type = "Expense", Icon = "utensils", Color = "#ff7a18", IsSystem = true },
            new Category { CategoryId = 2, Name = "Luong", Type = "Income", Icon = "wallet", Color = "#16a34a", IsSystem = true });

        dbContext.Transactions.AddRange(
            new TransactionEntry
            {
                TransactionEntryId = 1,
                UserId = 100,
                WalletId = 2,
                CategoryId = 1,
                Type = "Expense",
                Amount = 50_000m,
                Note = "An trua",
                TransactionDate = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new TransactionEntry
            {
                TransactionEntryId = 2,
                UserId = 100,
                WalletId = 1,
                CategoryId = 2,
                Type = "Income",
                Amount = 5_000_000m,
                Note = "Luong thang 3",
                TransactionDate = new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new TransactionEntry
            {
                TransactionEntryId = 3,
                UserId = 200,
                WalletId = 1,
                CategoryId = 1,
                Type = "Expense",
                Amount = 100_000m,
                Note = "User khac",
                TransactionDate = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            });

        dbContext.SaveChanges();
    }
}

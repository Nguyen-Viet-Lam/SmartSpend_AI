using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSpendAI.Controllers;
using SmartSpendAI.Models;
using SmartSpendAI.Models.Dtos.Admin;
using SmartSpendAI.Security;

namespace SmartSpendAI.Tests.Admin;

public sealed class AdminCategoriesControllerTests
{
    [Fact]
    public async Task CreateCategory_ReturnsCreated_AndPersistsAuditLog()
    {
        using var dbContext = CreateDbContext();
        var controller = CreateController(dbContext, userId: 99);

        var actionResult = await controller.CreateCategory(
            new AdminCategoryRequest
            {
                Name = "Di lai",
                Type = "Expense",
                Icon = "car",
                Color = "#00b894",
                IsSystem = false
            },
            CancellationToken.None);

        var created = Assert.IsType<CreatedResult>(actionResult.Result);
        var payload = Assert.IsType<AdminCategoryResponse>(created.Value);
        Assert.Equal("Di lai", payload.Name);
        Assert.Equal("Expense", payload.Type);

        var category = await dbContext.Categories.AsNoTracking().SingleAsync();
        Assert.Equal("Di lai", category.Name);

        var audit = await dbContext.AuditLogs.AsNoTracking().SingleAsync();
        Assert.Equal(99, audit.ActorUserId);
        Assert.Equal("AdminCategoryCreated", audit.Action);
    }

    [Fact]
    public async Task DeleteCategory_ReturnsBadRequest_WhenCategoryHasTransactions()
    {
        using var dbContext = CreateDbContext();
        await SeedCategoryWithTransactionAsync(dbContext, categoryId: 10);
        var controller = CreateController(dbContext, userId: 2);

        var actionResult = await controller.DeleteCategory(10, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.NotNull(badRequest.Value);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"admin-categories-tests-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }

    private static AdminCategoriesController CreateController(AppDbContext dbContext, int userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, AppRoles.SystemAdmin)
        };

        return new AdminCategoriesController(dbContext)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
                }
            }
        };
    }

    private static async Task SeedCategoryWithTransactionAsync(AppDbContext dbContext, int categoryId)
    {
        var role = new Role { RoleId = 1, RoleName = AppRoles.StandardUser };
        var user = new User
        {
            UserId = 1,
            Username = "u1",
            FullName = "User 1",
            Email = "u1@example.com",
            PasswordHash = "hash",
            RoleId = role.RoleId,
            CreatedAt = DateTime.UtcNow
        };
        var wallet = new Wallet
        {
            WalletId = 1,
            UserId = user.UserId,
            Name = "Cash",
            Type = "Cash",
            Balance = 100000,
            CreatedAt = DateTime.UtcNow
        };
        var category = new Category
        {
            CategoryId = categoryId,
            Name = "Cat 10",
            Type = "Expense",
            Icon = "circle",
            Color = "#48d1a0",
            IsSystem = false
        };
        var transaction = new TransactionEntry
        {
            TransactionEntryId = 1,
            UserId = user.UserId,
            WalletId = wallet.WalletId,
            CategoryId = categoryId,
            Type = "Expense",
            Amount = 50000,
            Note = "Test",
            TransactionDate = DateTime.UtcNow.Date,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Roles.Add(role);
        dbContext.Users.Add(user);
        dbContext.Wallets.Add(wallet);
        dbContext.Categories.Add(category);
        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync();
    }
}

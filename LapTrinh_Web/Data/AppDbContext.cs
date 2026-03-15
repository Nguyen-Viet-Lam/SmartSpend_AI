using LapTrinh_Web.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapTrinh_Web.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<TransactionRecord> Transactions => Set<TransactionRecord>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<BudgetPlan> Budgets => Set<BudgetPlan>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
    public DbSet<SystemNotification> Notifications => Set<SystemNotification>();
    public DbSet<SystemLog> Logs => Set<SystemLog>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureApplicationUser(modelBuilder);
        ConfigureUserProfile(modelBuilder);
        ConfigureWallet(modelBuilder);
        ConfigureTransaction(modelBuilder);
        ConfigureCategory(modelBuilder);
        ConfigureBudget(modelBuilder);
        ConfigureOtpCode(modelBuilder);
        ConfigureNotification(modelBuilder);
        ConfigureLog(modelBuilder);
    }

    private static void ConfigureApplicationUser(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ApplicationUser>();
        entity.ToTable("Users");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Email).IsRequired().HasMaxLength(255);
        entity.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);
        entity.Property(x => x.Role).IsRequired();
        entity.Property(x => x.Status).IsRequired();

        entity.HasIndex(x => x.Email).IsUnique();
    }

    private static void ConfigureUserProfile(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<UserProfile>();
        entity.ToTable("UserProfiles");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.DisplayName).IsRequired().HasMaxLength(150);
        entity.Property(x => x.AvatarUrl).HasMaxLength(500);

        entity.HasIndex(x => x.UserId).IsUnique();
        entity.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureWallet(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Wallet>();
        entity.ToTable("Wallets");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
        entity.Property(x => x.WalletType).IsRequired();
        entity.Property(x => x.Balance).IsRequired();

        entity.HasIndex(x => new { x.UserId, x.Name });
        entity.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureTransaction(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<TransactionRecord>();
        entity.ToTable("Transactions");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.TransactionType).IsRequired();
        entity.Property(x => x.Amount).IsRequired();
        entity.Property(x => x.Description).IsRequired().HasMaxLength(500);
        entity.Property(x => x.OccurredAtUtc).IsRequired();

        entity.HasIndex(x => new { x.UserId, x.OccurredAtUtc });
        entity.HasIndex(x => x.WalletId);
        entity.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne<Wallet>()
            .WithMany()
            .HasForeignKey(x => x.WalletId)
            .OnDelete(DeleteBehavior.Restrict);
        entity.HasOne<Category>()
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void ConfigureCategory(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Category>();
        entity.ToTable("Categories");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Name).IsRequired().HasMaxLength(120);
        entity.Property(x => x.Icon).HasMaxLength(100);

        entity.HasIndex(x => x.Name).IsUnique();
    }

    private static void ConfigureBudget(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<BudgetPlan>();
        entity.ToTable("Budgets");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.LimitAmount).IsRequired();
        entity.Property(x => x.Year).IsRequired();
        entity.Property(x => x.Month).IsRequired();

        entity.HasIndex(x => new { x.UserId, x.CategoryId, x.Year, x.Month }).IsUnique();
        entity.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne<Category>()
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureOtpCode(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<OtpCode>();
        entity.ToTable("OtpCodes");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Code).IsRequired().HasMaxLength(10);
        entity.Property(x => x.ExpiredAtUtc).IsRequired();

        entity.HasIndex(x => new { x.UserId, x.Code });
        entity.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureNotification(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<SystemNotification>();
        entity.ToTable("Notifications");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
        entity.Property(x => x.Message).IsRequired().HasMaxLength(1000);
        entity.Property(x => x.AlertLevel).IsRequired();

        entity.HasIndex(x => new { x.UserId, x.IsRead });
        entity.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureLog(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<SystemLog>();
        entity.ToTable("SystemLogs");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Level).IsRequired().HasMaxLength(50);
        entity.Property(x => x.Source).IsRequired().HasMaxLength(200);
        entity.Property(x => x.Message).IsRequired().HasMaxLength(2000);
    }
}

using Microsoft.EntityFrameworkCore;
using Web_Project.Security;

namespace Web_Project.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<EmailVerificationOtp> EmailVerificationOtps => Set<EmailVerificationOtp>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<TransactionEntry> Transactions => Set<TransactionEntry>();
        public DbSet<Budget> Budgets => Set<Budget>();
        public DbSet<BudgetAlert> BudgetAlerts => Set<BudgetAlert>();
        public DbSet<KeywordEntry> Keywords => Set<KeywordEntry>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<TransferRecord> Transfers => Set<TransferRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>()
                .HasIndex(x => x.RoleName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<EmailVerificationOtp>()
                .HasIndex(x => new { x.Email, x.Purpose, x.IsUsed, x.ExpiresAt });

            modelBuilder.Entity<Wallet>()
                .ToTable("Wallets");

            modelBuilder.Entity<Wallet>()
                .HasIndex(x => new { x.UserId, x.Name })
                .IsUnique();

            modelBuilder.Entity<Wallet>()
                .Property(x => x.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Category>()
                .ToTable("Categories");

            modelBuilder.Entity<Category>()
                .HasIndex(x => new { x.Name, x.Type })
                .IsUnique();

            modelBuilder.Entity<TransactionEntry>()
                .ToTable("Transactions");

            modelBuilder.Entity<TransactionEntry>()
                .HasIndex(x => new { x.UserId, x.TransactionDate });

            modelBuilder.Entity<TransactionEntry>()
                .HasIndex(x => new { x.WalletId, x.TransactionDate });

            modelBuilder.Entity<TransactionEntry>()
                .Property(x => x.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TransactionEntry>()
                .Property(x => x.AiConfidence)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Budget>()
                .ToTable("Budgets");

            modelBuilder.Entity<Budget>()
                .HasIndex(x => new { x.UserId, x.CategoryId, x.Month })
                .IsUnique();

            modelBuilder.Entity<Budget>()
                .Property(x => x.LimitAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BudgetAlert>()
                .ToTable("BudgetAlerts");

            modelBuilder.Entity<KeywordEntry>()
                .ToTable("Keywords");

            modelBuilder.Entity<KeywordEntry>()
                .HasIndex(x => x.Word);

            modelBuilder.Entity<AuditLog>()
                .ToTable("AuditLogs");

            modelBuilder.Entity<TransferRecord>()
                .ToTable("Transfers");

            modelBuilder.Entity<TransferRecord>()
                .Property(x => x.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Wallet>()
                .HasOne(x => x.User)
                .WithMany(x => x.Wallets)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransactionEntry>()
                .HasOne(x => x.User)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TransactionEntry>()
                .HasOne(x => x.Wallet)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.WalletId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TransactionEntry>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Budget>()
                .HasOne(x => x.User)
                .WithMany(x => x.Budgets)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Budget>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Budgets)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BudgetAlert>()
                .HasOne(x => x.User)
                .WithMany(x => x.BudgetAlerts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BudgetAlert>()
                .HasOne(x => x.Transaction)
                .WithMany()
                .HasForeignKey(x => x.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KeywordEntry>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Keywords)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditLog>()
                .HasOne(x => x.ActorUser)
                .WithMany(x => x.AuditLogs)
                .HasForeignKey(x => x.ActorUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TransferRecord>()
                .HasOne(x => x.FromWallet)
                .WithMany(x => x.OutgoingTransfers)
                .HasForeignKey(x => x.FromWalletId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TransferRecord>()
                .HasOne(x => x.ToWallet)
                .WithMany(x => x.IncomingTransfers)
                .HasForeignKey(x => x.ToWalletId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EmailVerificationOtp>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = AppRoles.StandardUser },
                new Role { RoleId = 2, RoleName = AppRoles.SystemAdmin });

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "An uong", Type = "Expense", Icon = "utensils", Color = "#ff7a18", IsSystem = true },
                new Category { CategoryId = 2, Name = "Di chuyen", Type = "Expense", Icon = "car", Color = "#00b894", IsSystem = true },
                new Category { CategoryId = 3, Name = "Giai tri", Type = "Expense", Icon = "film", Color = "#6c5ce7", IsSystem = true },
                new Category { CategoryId = 4, Name = "Hoa don", Type = "Expense", Icon = "receipt", Color = "#ff4d4f", IsSystem = true },
                new Category { CategoryId = 5, Name = "Mua sam", Type = "Expense", Icon = "bag", Color = "#0097e6", IsSystem = true },
                new Category { CategoryId = 6, Name = "Luong", Type = "Income", Icon = "wallet", Color = "#2ecc71", IsSystem = true },
                new Category { CategoryId = 7, Name = "Thuong", Type = "Income", Icon = "sparkles", Color = "#00cec9", IsSystem = true },
                new Category { CategoryId = 8, Name = "Khac", Type = "Expense", Icon = "circle", Color = "#95a5a6", IsSystem = true });

            modelBuilder.Entity<KeywordEntry>().HasData(
                new KeywordEntry { KeywordEntryId = 1, Word = "tra sua", CategoryId = 1, Weight = 10, IsActive = true },
                new KeywordEntry { KeywordEntryId = 2, Word = "an vat", CategoryId = 1, Weight = 8, IsActive = true },
                new KeywordEntry { KeywordEntryId = 3, Word = "com", CategoryId = 1, Weight = 7, IsActive = true },
                new KeywordEntry { KeywordEntryId = 4, Word = "xang", CategoryId = 2, Weight = 10, IsActive = true },
                new KeywordEntry { KeywordEntryId = 5, Word = "grab", CategoryId = 2, Weight = 9, IsActive = true },
                new KeywordEntry { KeywordEntryId = 6, Word = "xe bus", CategoryId = 2, Weight = 8, IsActive = true },
                new KeywordEntry { KeywordEntryId = 7, Word = "xem phim", CategoryId = 3, Weight = 10, IsActive = true },
                new KeywordEntry { KeywordEntryId = 8, Word = "karaoke", CategoryId = 3, Weight = 8, IsActive = true },
                new KeywordEntry { KeywordEntryId = 9, Word = "dien", CategoryId = 4, Weight = 8, IsActive = true },
                new KeywordEntry { KeywordEntryId = 10, Word = "nuoc", CategoryId = 4, Weight = 8, IsActive = true },
                new KeywordEntry { KeywordEntryId = 11, Word = "wifi", CategoryId = 4, Weight = 7, IsActive = true },
                new KeywordEntry { KeywordEntryId = 12, Word = "luong", CategoryId = 6, Weight = 10, IsActive = true },
                new KeywordEntry { KeywordEntryId = 13, Word = "thuong", CategoryId = 7, Weight = 10, IsActive = true });
        }
    }
}

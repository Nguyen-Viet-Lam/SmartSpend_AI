namespace SmartSpendAI.Models
{
    public class SmartSpendSeedOptions
    {
        public bool AutoApplyMigrations { get; set; }

        public bool Enabled { get; set; }

        public bool RecreateDatabaseOnMigrationFailure { get; set; }

        public bool ResetPasswordsOnSeed { get; set; } = true;

        public bool SeedDemoData { get; set; } = true;

        public string AdminUsername { get; set; } = "admin.smartspend";

        public string AdminFullName { get; set; } = "SmartSpend Admin";

        public string AdminEmail { get; set; } = "admin@smartspend.local";

        public string AdminPassword { get; set; } = "Admin123!";

        public string DemoUsername { get; set; } = "demo.smartspend";

        public string DemoFullName { get; set; } = "Demo SmartSpend";

        public string DemoEmail { get; set; } = "demo@smartspend.local";

        public string DemoPassword { get; set; } = "Demo123!";
    }
}

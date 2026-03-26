namespace SmartSpendAI.Services.Setup
{
    public interface ISmartSpendDataSeeder
    {
        Task SeedAsync(CancellationToken cancellationToken);
    }
}

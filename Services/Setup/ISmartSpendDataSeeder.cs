namespace Web_Project.Services.Setup
{
    public interface ISmartSpendDataSeeder
    {
        Task SeedAsync(CancellationToken cancellationToken);
    }
}

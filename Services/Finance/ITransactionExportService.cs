using SmartSpendAI.Models.Dtos.Finance;

namespace SmartSpendAI.Services.Finance
{
    public interface ITransactionExportService
    {
        Task<byte[]> ExportAsync(int userId, TransactionExportFilter filter, CancellationToken cancellationToken);
    }
}

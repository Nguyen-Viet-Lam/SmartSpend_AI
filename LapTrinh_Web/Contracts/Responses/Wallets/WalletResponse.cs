using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Contracts.Responses.Wallets;

public sealed class WalletResponse
{
    public Guid WalletId { get; set; }
    public string Name { get; set; } = string.Empty;
    public WalletType WalletType { get; set; } = WalletType.Cash;
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
}
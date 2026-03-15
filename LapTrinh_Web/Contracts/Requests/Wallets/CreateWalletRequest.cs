using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Contracts.Requests.Wallets;

public sealed class CreateWalletRequest
{
    public string Name { get; set; } = string.Empty;
    public WalletType WalletType { get; set; } = WalletType.Cash;
    public decimal InitialBalance { get; set; }
}
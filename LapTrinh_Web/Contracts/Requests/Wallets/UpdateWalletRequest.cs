using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Contracts.Requests.Wallets;

public sealed class UpdateWalletRequest
{
    public string Name { get; set; } = string.Empty;
    public WalletType WalletType { get; set; } = WalletType.Cash;
    public bool IsActive { get; set; } = true;
}
using LapTrinh_Web.Core.Enums;

namespace LapTrinh_Web.Core.Entities;

public class Wallet : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public WalletType WalletType { get; set; } = WalletType.Cash;
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;
}
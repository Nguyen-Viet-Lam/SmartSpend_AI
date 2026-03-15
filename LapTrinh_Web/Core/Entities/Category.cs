namespace LapTrinh_Web.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
}
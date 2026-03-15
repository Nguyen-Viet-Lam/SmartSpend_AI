namespace LapTrinh_Web.Contracts.Requests.Admin;

public sealed class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public bool IsActive { get; set; } = true;
}
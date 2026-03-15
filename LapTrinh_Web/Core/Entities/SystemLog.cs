namespace LapTrinh_Web.Core.Entities;

public class SystemLog : BaseEntity
{
    public string Level { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
}
namespace LapTrinh_Web.Contracts.Responses.Dashboard;

public sealed class SpendingForecastResponse
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal PredictedTotalExpense { get; set; }
}
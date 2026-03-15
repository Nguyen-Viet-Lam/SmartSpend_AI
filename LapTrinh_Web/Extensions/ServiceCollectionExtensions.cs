using LapTrinh_Web.Core.Entities;
using LapTrinh_Web.Services.Background;
using LapTrinh_Web.Services.Implementations;
using LapTrinh_Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LapTrinh_Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmartSpendSkeletonServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<ICategorySuggestionService, CategorySuggestionService>();
        services.AddScoped<IForecastService, ForecastService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddSingleton<IEmailService, SmtpEmailService>();
        services.AddSingleton<IEmailDispatchQueue, EmailDispatchQueue>();
        services.AddHostedService<EmailDispatchBackgroundService>();
        services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();

        return services;
    }
}

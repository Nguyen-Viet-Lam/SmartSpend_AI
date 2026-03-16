using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Web_Project.Models;
using Web_Project.Security;
using Web_Project.Services.AI;
using Web_Project.Services.Auth;
using Web_Project.Services.Email;
using Web_Project.Services.Otp;
using Web_Project.Services.Realtime;
using Web_Project.Services.Setup;
using Web_Project.Services.Users;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".keys")))
    .SetApplicationName("SmartSpendAI");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<EmailOtpSettings>(builder.Configuration.GetSection("EmailOtp"));
builder.Services.Configure<SmartSpendSeedOptions>(builder.Configuration.GetSection("SmartSpendSeed"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
{
    jwtSettings.SecretKey = "CHANGE_THIS_TO_A_STRONG_SECRET_KEY_AT_LEAST_32_CHARS";
}

var jwtSigningMaterial = JwtSigningMaterial.Create(jwtSettings, builder.Environment.ContentRootPath);
builder.Services.AddSingleton(jwtSigningMaterial);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = jwtSigningMaterial.ValidationKey,
            ValidAlgorithms = [jwtSigningMaterial.Algorithm],
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/budget-alerts"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IEmailOtpService, EmailOtpService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISmartInputService, SmartInputService>();
builder.Services.AddScoped<ISmartSpendDataSeeder, SmartSpendDataSeeder>();

var app = builder.Build();

if (jwtSigningMaterial.IsAsymmetric && jwtSigningMaterial.IsEphemeral)
{
    app.Logger.LogWarning("JWT asymmetric mode is using an auto-generated in-memory key pair. Tokens will be invalid after restart.");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

await EnsureSmartSpendSetupAsync(app);

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<BudgetAlertsHub>("/hubs/budget-alerts");
app.MapStaticAssets();

app.MapGet("/", () => Results.Redirect("/home/index.html"));
app.MapGet("/home", () => Results.Redirect("/home/index.html"));
app.MapGet("/error", () => Results.Problem("Da xay ra loi khong mong muon."));

app.Run();

static async Task EnsureSmartSpendSetupAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<SmartSpendSeedOptions>>().Value;
    if (!options.AutoApplyMigrations && !options.Enabled)
    {
        return;
    }

    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (options.AutoApplyMigrations)
        {
            await ApplyMigrationsWithRecoveryAsync(app, dbContext, options);
            app.Logger.LogInformation("SmartSpend migrations applied successfully.");
        }

        if (options.Enabled)
        {
            var seeder = scope.ServiceProvider.GetRequiredService<ISmartSpendDataSeeder>();
            await seeder.SeedAsync(CancellationToken.None);
            app.Logger.LogInformation("SmartSpend development seed completed.");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "SmartSpend setup skipped because database migration/seed could not be completed.");
    }
}

static async Task ApplyMigrationsWithRecoveryAsync(
    WebApplication app,
    AppDbContext dbContext,
    SmartSpendSeedOptions options)
{
    try
    {
        await dbContext.Database.MigrateAsync();
    }
    catch (Exception ex) when (options.RecreateDatabaseOnMigrationFailure)
    {
        app.Logger.LogWarning(ex, "Migration failed. Recreating SmartSpend development database from scratch.");
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }
}

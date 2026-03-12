using Microsoft.EntityFrameworkCore;
using Wed_Project.Models;
using Wed_Project.Services.AI;
using Wed_Project.Services.Auth;
using Wed_Project.Services.Content;
using Wed_Project.Services.Email;
using Wed_Project.Services.Otp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<EmailOtpSettings>(builder.Configuration.GetSection("EmailOtp"));
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("Gemini"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IEmailOtpService, EmailOtpService>();
builder.Services.AddHttpClient<IGeminiSummaryService, GeminiSummaryService>();
builder.Services.AddScoped<ISummaryProcessingService, SummaryProcessingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapStaticAssets();

app.MapGet("/", () => Results.Redirect("/home/index.html"));
app.MapGet("/home", () => Results.Redirect("/home/index.html"));
app.MapGet("/error", () => Results.Problem("Đã xảy ra lỗi không mong muốn."));

app.Run();

using TradingDashboard.Services;
using TradingDashboard.Shared;

var builder = WebApplication.CreateBuilder(args);

// Access configuration
var config = builder.Configuration;
var apiBaseUrl = config["ApiBaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException("ApiBaseUrl is not set in configuration.");
}
Console.WriteLine($"ApiBaseUrl: {apiBaseUrl}"); // For debugging purposes

// Inject HttpClient with base URL from config
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("https://fxai2-hrgzeve9dka0aqg3.canadacentral-01.azurewebsites.net"); // Replace with the actual base address
});

// Register services
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSingleton<PatternStatsEngine>();
builder.Services.AddSingleton<StrategyExecutionService>();
builder.Services.AddSingleton<CandleLoader>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

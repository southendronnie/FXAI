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


builder.Services.AddHttpClient<ApiClient>(client =>
{
  client.BaseAddress = new Uri(apiBaseUrl);
});

// Register services
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSingleton<PatternStatsEngine>();
builder.Services.AddSingleton<StrategyExecutionService>();
builder.Services.AddSingleton<CandleLoader>();
builder.Services.AddLogging();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

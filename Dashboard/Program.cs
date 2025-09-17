using TradingDashboard.Components;
using TradingDashboard.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(sp =>
{
  var config = builder.Configuration;
  var baseUrl = config["ApiBaseUrl"];
  return new HttpClient { BaseAddress = new Uri(baseUrl!) };

});


// Add services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<PatternStatsEngine>();
builder.Services.AddSingleton<StrategyExecutionService>();
builder.Services.AddSingleton<CandleLoader>();
builder.Services.AddScoped<ApiClient>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

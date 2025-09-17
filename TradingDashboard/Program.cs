using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using TradingDashboard;
using TradingDashboard.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Set your backend API base URL here
builder.Services.AddScoped(sp => new HttpClient
{
  BaseAddress = new Uri("https://fxai2-hrgzeve9dka0aqg3.canadacentral-01.azurewebsites.net")
});

// Register your custom API client
builder.Services.AddScoped<ApiClient>();

await builder.Build().RunAsync();
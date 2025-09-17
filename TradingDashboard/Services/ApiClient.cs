using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TradingDashboard.Models;

namespace TradingDashboard.Services;

public class ApiClient
{
  private readonly HttpClient _http;

  public ApiClient(HttpClient http)
  {
    _http = http;
  }

  public async Task<PriceTick?> GetLatestTick()
  {
    var status = await _http.GetFromJsonAsync<StreamStatus>("/stream/status");
    return status?.LatestTick;
  }

  public async Task<List<Candle>> GetCandles(string timeframe, DateTime start, DateTime end)
  {
    var url = $"/api/candles/{timeframe}?start={start:o}&end={end:o}";
    var candles = await _http.GetFromJsonAsync<List<Candle>>(url);
    return candles ?? new List<Candle>();
  }

  public async Task<HealthStatus?> GetHealth()
  {
    return await _http.GetFromJsonAsync<HealthStatus>("/health");
  }

  public async Task<bool> SubmitOrder(string side)
  {
    var payload = new { side = side, instrument = "EUR/USD", quantity = 1000 };
    var response = await _http.PostAsJsonAsync("/api/order", payload);
    return response.IsSuccessStatusCode;
  }
}
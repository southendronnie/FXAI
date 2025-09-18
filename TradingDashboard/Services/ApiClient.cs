using System.Net.Http;
using System.Net.Http.Json;
using TradingDashboard.Model;
using TradingDashboard.Models;
using static TradingDashboard.Model.PatternStatsEngine;

namespace TradingDashboard.Services
{
  public class ApiClient
  {
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
      _http = http;
    }

    public async Task<List<TradeResult>> GetTradesAsync(string strategyId)
    {
      var endpoint = $"api/trades/{strategyId}";
      try
      {
        var results = await _http.GetFromJsonAsync<List<TradeResult>>(endpoint);
        return results ?? new List<TradeResult>();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error fetching trades: {ex.Message}");
        return new List<TradeResult>();
      }
    }

    public async Task<PatternStats> GetPatternStatsAsync(string strategyId)
    {
      var endpoint = $"api/patterns/{strategyId}";
      try
      {
        var stats = await _http.GetFromJsonAsync<PatternStats>(endpoint);
        return stats ?? new PatternStats();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error fetching pattern stats: {ex.Message}");
        return new PatternStats();
      }
    }

    public async Task<List<Candle>> GetCandlesAsync(string instrument, string timeframe)
    {
      var endpoint = $"api/candles/{instrument}/{timeframe}";
      try
      {
        var candles = await _http.GetFromJsonAsync<List<Candle>>(endpoint);
        return candles ?? new List<Candle>();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error fetching candles: {ex.Message}");
        return new List<Candle>();
      }
    }

    public async Task<PriceTick?> GetLatestTickAsync(string instrument)
    {
      var endpoint = $"api/ticks/latest/{instrument}";
      try
      {
        return await _http.GetFromJsonAsync<PriceTick>(endpoint);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error fetching latest tick: {ex.Message}");
        return null;
      }
    }

    public async Task<HealthStatus> GetHealthAsync()
    {
      var endpoint = $"api/health";
      try
      {
        var health = await _http.GetFromJsonAsync<HealthStatus>(endpoint);
        return health ?? new HealthStatus { IsHealthy = false, Message = "No response", CheckedAt = DateTime.UtcNow };
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error fetching health status: {ex.Message}");
        return new HealthStatus { IsHealthy = false, Message = "Exception thrown", CheckedAt = DateTime.UtcNow };
      }
    }

    public async Task<StreamStatus> GetStreamStatusAsync()
    {
      var endpoint = $"api/stream/status";
      try
      {
        var status = await _http.GetFromJsonAsync<StreamStatus>(endpoint);
        return status ?? new StreamStatus { IsConnected = false, Latency = TimeSpan.Zero, Source = "Unknown" };
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error fetching stream status: {ex.Message}");
        return new StreamStatus { IsConnected = false, Latency = TimeSpan.Zero, Source = "Exception" };
      }
    }
    public Task<bool> SubmitOrderAsync(string instrument)
    {
      var order = new OrderRequest
      {
        Instrument = instrument,
        Side = "buy",
        Units = 1000,
        Type = "market",
        StrategyId = "default"
      };
      return SubmitOrderAsync(order);
    }
    public async Task<bool> SubmitOrderAsync(OrderRequest order)
    {
      var endpoint = "api/orders/submit";
      try
      {
        var response = await _http.PostAsJsonAsync(endpoint, order);
        return response.IsSuccessStatusCode;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error submitting order: {ex.Message}");
        return false;
      }
    }
  }
}
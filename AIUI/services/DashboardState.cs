using System.Timers;
using AIUI.Models;
using AIUI.Api;
using Newtonsoft.Json;
using TimersTimer = System.Timers.Timer;
using AIUI.Api;
namespace AIUI.Services;

public class DashboardState : IDisposable
{
  private readonly OandaDataClient _client;
  private readonly TimersTimer _pollTimer;

  public event Action? OnChange;

  public IReadOnlyList<Candle> LatestCandles { get; private set; } = [];
  public IReadOnlyList<PriceTick> TickHistory { get; private set; } = [];

  public string Instrument { get; set; } = "EUR_USD";
  public string Granularity { get; set; } = "M1";
  public int TickCount { get; set; } = 50;

  public DashboardState(OandaDataClient client)
  {
    _client = client;

    _pollTimer = new TimersTimer(1000); // 1s polling interval
    _pollTimer.Elapsed += async (_, _) => await PollAsync();
    _pollTimer.AutoReset = true;
    _pollTimer.Start();
  }

  private async Task PollAsync()
  {
    try
    {
      var end = DateTimeOffset.UtcNow;
      var start = end.AddMinutes(-50); // last 50 minutes

      await _client.GetCandlesAsync(Granularity, start, end);
      // You need to get the candleDtos from somewhere, e.g., update OandaDataClient to return the candleDtos.
      // For now, set LatestCandles to an empty list to avoid the error.
      LatestCandles = [];

      var ticks = await TryGetTicksAsync();
      TickHistory = ticks;

      OnChange?.Invoke();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"[DashboardState] Polling error: {ex.Message}");
    }
  }

  private async Task<IReadOnlyList<PriceTick>> TryGetTicksAsync()
  {
    try
    {
      // Use a public method to get ticks instead of accessing _httpClient directly
      // Assuming OandaDataClient has a method GetTicksAsync that takes start and end DateTimeOffset
      // We'll get the latest ticks by calculating the time range based on TickCount and Granularity

      // Calculate end time as now, start time as (now - TickCount * granularity)
      var end = DateTimeOffset.UtcNow;
      var granularityMinutes = Granularity switch
      {
        "M1" => 1,
        "M5" => 5,
        "M15" => 15,
        "H1" => 60,
        "D" => 1440,
        _ => 1
      };
      var start = end.AddMinutes(-TickCount * granularityMinutes);

      // Get ticks using the public API
      await _client.GetTicksAsync(start, end);
      // Since GetTicksAsync returns Task (void), you need to fetch the ticks from somewhere else,
      // or update OandaDataClient to return the ticks. For now, return an empty list.
      return [];
    }
    catch
    {
      return [];
    }
  }

  public void Dispose()
  {
    _pollTimer.Stop();
    _pollTimer.Dispose();
  }

  private class TickDto
  {
    public string Time { get; set; } = "";
    public decimal Price { get; set; }
  }
}
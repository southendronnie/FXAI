using System.Timers;
using AIUI.Models;
using AIUI.Api;

namespace AIUI.Services;

public class DashboardState : IDisposable
{
    private readonly OandaDataClient _client;
    private readonly Timer _pollTimer;

    public event Action? OnChange;

    public IReadOnlyList<Candle> LatestCandles { get; private set; } = [];
    public IReadOnlyList<PriceTick> TickHistory { get; private set; } = [];

    public DashboardState(OandaDataClient client)
    {
        _client = client;

        _pollTimer = new Timer(1000); // 1s polling interval
        _pollTimer.Elapsed += async (_, _) => await PollAsync();
        _pollTimer.Start();
    }

    private async Task PollAsync()
    {
        try
        {
            var candles = await _client.GetCandlesAsync("EUR_USD", "M1", 50);
            var ticks = await _client.GetRecentTicksAsync("EUR_USD", 50);

            LatestCandles = candles;
            TickHistory = ticks;

            OnChange?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardState] Polling error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _pollTimer.Stop();
        _pollTimer.Dispose();
    }
}
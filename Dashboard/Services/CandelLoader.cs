using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Candle
{
  public DateTime Time { get; set; }
  public decimal Open { get; set; }
  public decimal High { get; set; }
  public decimal Low { get; set; }
  public decimal Close { get; set; }
  public long Volume { get; set; }
}

public class CandleLoader
{
  public Task<List<Candle>> LoadAsync(string instrument, string timeframe)
  {
    // Stubbed sample data — replace with real loader
    var candles = new List<Candle>();
    var now = DateTime.UtcNow;

    for (int i = 0; i < 500; i++)
    {
      var basePrice = 1.1000m + i * 0.0001m;
      candles.Add(new Candle
      {
        Time = now.AddMinutes(-i),
        Open = basePrice,
        High = basePrice + 0.0005m,
        Low = basePrice - 0.0005m,
        Close = basePrice + (i % 2 == 0 ? 0.0002m : -0.0002m),
        Volume = 1000 + i
      });
    }

    candles.Reverse(); // oldest first
    return Task.FromResult(candles);
  }
}
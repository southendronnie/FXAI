using TradingDashboard.Models;
using TradingDashboard.Models;

namespace TradingDashboard.Services
{
  public class CandleLoader
  {
    public List<Candle> BuildCandles(List<PriceTick> ticks, TimeSpan interval)
    {
      var grouped = ticks
          .GroupBy(t => new DateTime((t.Timestamp.Ticks / interval.Ticks) * interval.Ticks))
          .OrderBy(g => g.Key);

      var candles = new List<Candle>();

      foreach (var group in grouped)
      {
        var prices = group.Select(t => (t.Bid + t.Ask) / 2).ToList();
        candles.Add(new Candle
        {
          Time = group.Key,
          Open = prices.First(),
          High = prices.Max(),
          Low = prices.Min(),
          Close = prices.Last(),
          Volume = prices.Count
        });
      }

      return candles;
    }
  }
}
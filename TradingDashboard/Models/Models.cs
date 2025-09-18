using TradingDashboard.Models;

namespace TradingDashboard.Model
{
    public class PriceTick
    {
      public DateTime Timestamp { get; set; }
      public decimal Bid { get; set; }
      public decimal Ask { get; set; }

      public decimal Mid => (Bid + Ask) / 2;
      public TimeSpan Age => DateTime.UtcNow - Timestamp;
    }

  public class Candle
  {
    public DateTime Time { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public int Volume { get; set; }
  }

  public class StreamStatus
  {
    public bool IsConnected { get; set; }
    public TimeSpan Latency { get; set; }
    public string Source { get; set; } = string.Empty;
  }

  public class HealthStatus
  {
    public bool IsHealthy { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CheckedAt { get; set; }
  }

  // Models/PatternStats.cs or Models/Diagnostics.cs
  public class DiagnosticItem
  {
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Status { get; set; } = "Unknown"; // Healthy, Warning, Critical
  }

  public class PatternStats
  {
    public int TotalTrades { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public decimal WinRate => TotalTrades > 0 ? (decimal)Wins / TotalTrades : 0;
    public decimal AveragePnL { get; set; }
    public Dictionary<string, int> PatternFrequency { get; set; } = new();
  }

  public class PatternLibrary
  {
    public List<string> KnownPatterns { get; set; } = new();

    public bool IsMatch(string pattern)
    {
      return KnownPatterns.Contains(pattern);
    }
  }

  public class PatternStatsEngine
  {
    public PatternStats ComputeStats(List<TradeResult> trades)
    {
      var stats = new PatternStats
      {
        TotalTrades = trades.Count,
        Wins = trades.Count(t => t.IsWin),
        Losses = trades.Count(t => !t.IsWin),
        AveragePnL = trades.Any() ? trades.Average(t => t.ProfitLoss) : 0
      };

      foreach (var trade in trades)
      {
        var pattern = trade.StrategyId; // Simplified pattern key
        if (!stats.PatternFrequency.ContainsKey(pattern))
          stats.PatternFrequency[pattern] = 0;
        stats.PatternFrequency[pattern]++;
      }

      return stats;
    }
    public class OrderRequest
    {
      public string Instrument { get; set; } = string.Empty;
      public string Side { get; set; } = "buy"; // or "sell"
      public decimal Units { get; set; }
      public decimal? Price { get; set; } // Optional for limit orders
      public string Type { get; set; } = "market"; // market, limit, stop
      public string StrategyId { get; set; } = string.Empty;
  }
}

}
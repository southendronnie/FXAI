using TradingDashboard.Models;

public class PatternStatsEngine
{
  public PatternStats ComputeStats(IEnumerable<TradeResult> trades, string patternId, string strategyId)
  {
    var tradeList = trades.ToList();
    int total = tradeList.Count;
    if (total == 0)
    {
      return new PatternStats
      {
        Timestamp = DateTime.UtcNow,
        PatternId = patternId,
        StrategyId = strategyId,
        TotalTrades = 0,
        WinRate = 0,
        MaxDrawdown = 0,
        AverageReturn = 0,
        NetPnL = 0,
        SharpeRatio = 0
      };
    }

    double netPnL = tradeList.Sum(t => (double)t.Profit);
    double avgReturn = tradeList.Average(t => (double)t.Profit);
    double winRate = tradeList.Count(t => t.Profit > 0) / (double)total;

    double maxDrawdown = CalculateMaxDrawdown(tradeList);
    double sharpe = CalculateSharpeRatio(tradeList, avgReturn);

    return new PatternStats
    {
      Timestamp = DateTime.UtcNow,
      PatternId = patternId,
      StrategyId = strategyId,
      TotalTrades = total,
      WinRate = winRate,
      MaxDrawdown = maxDrawdown,
      AverageReturn = avgReturn,
      NetPnL = netPnL,
      SharpeRatio = sharpe
    };
  }

  private double CalculateMaxDrawdown(List<TradeResult> trades)
  {
    double peak = 0;
    double trough = 0;
    double maxDrawdown = 0;
    double cumulative = 0;

    foreach (var trade in trades)
    {
      cumulative += (double)trade.Profit;
      if (cumulative > peak)
      {
        peak = cumulative;
        trough = cumulative;
      }
      if (cumulative < trough)
      {
        trough = cumulative;
        maxDrawdown = Math.Min(maxDrawdown, trough - peak);
      }
    }

    return maxDrawdown;
  }

  private double CalculateSharpeRatio(List<TradeResult> trades, double avgReturn)
  {
    if (trades.Count < 2) return 0;

    double variance = trades.Select(t => Math.Pow((double)t.Profit - avgReturn, 2)).Average();
    double stdDev = Math.Sqrt(variance);
    return stdDev == 0 ? 0 : avgReturn / stdDev;
  }
}
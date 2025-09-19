public class PatternStats
{
  public DateTime Timestamp { get; set; }

  // Core performance metrics
  public double WinRate { get; set; }           // e.g. 0.65 = 65%
  public double MaxDrawdown { get; set; }       // e.g. -0.12 = -12%
  public double AverageReturn { get; set; }     // e.g. 0.03 = +3%
  public double NetPnL { get; set; }            // e.g. 1250.75 = total profit/loss
  public int TotalTrades { get; set; }          // e.g. 42 trades evaluated
  public double SharpeRatio { get; set; }       // risk-adjusted return

  // Optional metadata
  public string PatternId { get; set; } = string.Empty;
  public string StrategyId { get; set; } = string.Empty;

  // Optional diagnostics
  public string Source { get; set; } = string.Empty; // e.g. "Backtest", "LiveStream"
  public string Notes { get; set; } = string.Empty;
}
public class TradeResult
{
  public DateTime EntryTime { get; set; }
  public DateTime ExitTime { get; set; }

  public decimal EntryPrice { get; set; }
  public decimal ExitPrice { get; set; }

  public TradeDirection Direction { get; set; } // Buy or Sell
  public decimal Units { get; set; }

  public decimal Profit { get; set; }           // Net PnL for this trade

  public string StrategyId { get; set; } = string.Empty;
  public string PatternId { get; set; } = string.Empty;

  public string Instrument { get; set; } = string.Empty;
  public string Notes { get; set; } = string.Empty;
  public decimal ProfitLoss { get; set; }

  public bool IsWin => ProfitLoss > 0;


}
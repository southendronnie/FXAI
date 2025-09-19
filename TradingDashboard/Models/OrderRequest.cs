public class OrderRequest
{
  public DateTime Timestamp { get; set; } = DateTime.UtcNow;

  // Trade metadata
  public string Instrument { get; set; } = string.Empty;   // e.g. "EUR/USD"
  public TradeDirection Direction { get; set; }            // Buy or Sell
  public decimal Units { get; set; }                       // e.g. 10000
  public decimal Price { get; set; }                       // Optional for limit orders

  // Order type
  public OrderType Type { get; set; } = OrderType.Market;  // Market, Limit, Stop

  // Strategy and pattern metadata
  public string StrategyId { get; set; } = string.Empty;
  public string PatternId { get; set; } = string.Empty;

  // Optional execution flags
  public bool IsStopLossEnabled { get; set; } = false;
  public decimal? StopLossPrice { get; set; }

  public bool IsTakeProfitEnabled { get; set; } = false;
  public decimal? TakeProfitPrice { get; set; }

  // Optional diagnostics
  public string SourcePanel { get; set; } = string.Empty;
  public string Notes { get; set; } = string.Empty;

  public TradeDirection Side
  {
    get => Direction;
    set => Direction = value;
  }

}
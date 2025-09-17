public class TradeResult
{
  public DateTime EntryTime { get; set; }
  public DateTime ExitTime { get; set; }
  public decimal RawPnL { get; set; }
  public decimal NetPnL { get; set; }
  public decimal Cost { get; set; }
}
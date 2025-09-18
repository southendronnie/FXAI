using Microsoft.Extensions.Hosting;

namespace TradingDashboard.Models
{
  public class TradeResult
  {
    public string StrategyId { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
    public DateTime ExitTime { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal ExitPrice { get; set; }
    public decimal ProfitLoss { get; set; }
    public decimal RawPnL => ExitPrice - EntryPrice;
    public decimal NetPnL => ProfitLoss; // Already adjusted

    public bool IsWin => ProfitLoss > 0;
    public TimeSpan Duration => ExitTime - EntryTime;
    public decimal ReturnRate => EntryPrice > 0 ? (ExitPrice - EntryPrice) / EntryPrice : 0;
  }
}

namespace TradingDashboard.Models
{
  public class PriceTick
  {
    public DateTime Timestamp { get; set; }
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }

    public decimal Mid => (Bid + Ask) / 2;
    public TimeSpan Age => DateTime.UtcNow - Timestamp;
  }
}

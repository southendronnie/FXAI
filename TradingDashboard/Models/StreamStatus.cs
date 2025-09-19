namespace TradingDashboard.Models
{
  public class StreamStatus
  {
    public bool IsConnected { get; set; }
    public TimeSpan Latency { get; set; }
    public string Source { get; set; } = string.Empty;
  }



}

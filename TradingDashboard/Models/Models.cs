using System;

namespace TradingDashboard.Models;

public class PriceTick
{
  public DateTime Time { get; set; }
  public decimal Bid { get; set; }
  public decimal Ask { get; set; }
  public decimal Mid => (Bid + Ask) / 2;
}

public class Candle
{
  public DateTime Time { get; set; }
  public decimal Open { get; set; }
  public decimal High { get; set; }
  public decimal Low { get; set; }
  public decimal Close { get; set; }
}

public class StreamStatus
{
  public DateTime LastTickTime { get; set; }
  public PriceTick? LatestTick { get; set; }
  public Candle? Latest1m { get; set; }
  public Candle? Latest5m { get; set; }
}

public class HealthStatus
{
  public string Status { get; set; } = "unknown";
  public DateTime? LastTickTime { get; set; }
  public int? SecondsSinceLastTick { get; set; }
  public int UptimeSeconds { get; set; }
}
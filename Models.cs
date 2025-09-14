using MemoryPack;
using System.ComponentModel.DataAnnotations;

namespace FXAI2.Models;

/// <summary>
/// Represents a single price tick from OANDA.
/// </summary>
[MemoryPackable]
public partial class PriceTick
{
  /// <summary>UTC timestamp of the tick.</summary>
  [Required]
  public DateTime Time { get; set; }

  /// <summary>Bid price at this tick.</summary>
  [Required]
  public decimal Bid { get; set; }

  /// <summary>Ask price at this tick.</summary>
  [Required]
  public decimal Ask { get; set; }

  /// <summary>Midpoint between bid and ask.</summary>
  public decimal Mid => (Bid + Ask) / 2;
}

/// <summary>
/// Represents a completed OHLC candle for a given timeframe.
/// </summary>
[MemoryPackable]
public partial class Candle
{
  /// <summary>Start time of the candle bucket (UTC).</summary>
  [Required]
  public DateTime Time { get; set; }

  /// <summary>Opening price.</summary>
  [Required]
  public decimal Open { get; set; }

  /// <summary>Highest price during the candle period.</summary>
  [Required]
  public decimal High { get; set; }

  /// <summary>Lowest price during the candle period.</summary>
  [Required]
  public decimal Low { get; set; }

  /// <summary>Closing price.</summary>
  [Required]
  public decimal Close { get; set; }
}
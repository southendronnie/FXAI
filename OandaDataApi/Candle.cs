using MemoryPack;
using System.ComponentModel.DataAnnotations;


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

using MemoryPack;
using System.ComponentModel.DataAnnotations;
/// <summary>
/// Represents a single price tick from OANDA, including bid, ask, and computed mid price.
/// </summary>
[MemoryPackable]
public partial class PriceTick
{
  /// <summary>UTC timestamp of the tick.</summary>
  public DateTime Time { get; set; }

  /// <summary>Bid price at this tick.</summary>
  public decimal Bid { get; set; }

  /// <summary>Ask price at this tick.</summary>
  public decimal Ask { get; set; }

  /// <summary>Midpoint between bid and ask.</summary>
  public decimal Mid => (Bid + Ask) / 2;
}
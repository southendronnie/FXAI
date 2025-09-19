using MemoryPack;

[MemoryPackable]
public partial class PackedTradeResult
{
  public DateTime Timestamp { get; set; }
  public decimal Profit { get; set; }
  public string StrategyId { get; set; } = string.Empty;
}

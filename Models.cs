// --- Models (must come first) ---
using MemoryPack;
using System.Net.Http.Headers;
using System.Text.Json;


[MemoryPackable]
public partial class Candle
{
    public DateTime Time { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
}

[MemoryPackable]
public partial class PriceTick
{
    public DateTime Time { get; set; }
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public decimal Mid => (Bid + Ask) / 2;
}
namespace AIUI.Models;

public readonly struct Candle
{
    public DateTime Time { get; init; }
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
    public long Volume { get; init; }

    public Candle(DateTime time, decimal open, decimal high, decimal low, decimal close, long volume)
    {
        Time = time;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
    }
}
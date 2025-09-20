namespace AIUI.Models;

public readonly struct PriceTick
{
    public DateTime Time { get; init; }
    public decimal Price { get; init; }

    public PriceTick(DateTime time, decimal price)
    {
        Time = time;
        Price = price;
    }
}
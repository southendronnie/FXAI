using System;
using TradingDashboard.Models;

public static class PatternLibrary
{
  public static bool IsBullishEngulfing(Candle prev, Candle current)
  {
    return prev.Close < prev.Open && // previous candle is bearish
           current.Close > current.Open && // current candle is bullish
           current.Open < prev.Close &&
           current.Close > prev.Open;
  }

  public static bool IsBearishEngulfing(Candle prev, Candle current)
  {
    return prev.Close > prev.Open && // previous candle is bullish
           current.Close < current.Open && // current candle is bearish
           current.Open > prev.Close &&
           current.Close < prev.Open;
  }

  public static bool IsHammer(Candle candle)
  {
    var body = Math.Abs(candle.Close - candle.Open);
    var lowerWick = candle.Open < candle.Close
        ? candle.Open - candle.Low
        : candle.Close - candle.Low;
    var upperWick = candle.High - Math.Max(candle.Open, candle.Close);

    return lowerWick > body * 2 && upperWick < body;
  }

  public static bool IsDoji(Candle candle)
  {
    var body = Math.Abs(candle.Close - candle.Open);
    var range = candle.High - candle.Low;
    return body < range * 0.1m;
  }

  // Add more patterns here as needed
}
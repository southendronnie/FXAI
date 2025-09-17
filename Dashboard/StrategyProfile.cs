using System;
using System.Collections.Generic;

public class StrategyProfile
{
  public string PatternType { get; set; } = "";
  public decimal EntryOffset { get; set; } = 0;     // Optional offset from entry price
  public int Horizon { get; set; } = 5;              // Number of candles to hold
  public decimal StopLoss { get; set; } = 0;         // Optional SL logic (not enforced here)
  public decimal TakeProfit { get; set; } = 0;       // Optional TP logic (not enforced here)
  public decimal ConfidenceScore { get; set; } = 0;  // Derived from stats

  public List<TradeResult> Execute(List<Candle> candles, OandaCostModel costModel, int units)
  {
    var results = new List<TradeResult>();

    for (int i = 1; i < candles.Count - Horizon; i++)
    {
      var prev = candles[i - 1];
      var current = candles[i];
      var entry = candles[i];
      var exit = candles[i + Horizon];

      bool match = PatternType switch
      {
        "BullishEngulfing" => PatternLibrary.IsBullishEngulfing(prev, current),
        "BearishEngulfing" => PatternLibrary.IsBearishEngulfing(prev, current),
        "Hammer" => PatternLibrary.IsHammer(current),
        "Doji" => PatternLibrary.IsDoji(current),
        _ => false
      };

      if (!match) continue;

      var rawPnl = (exit.Close - entry.Close + EntryOffset) * units;
      var cost = costModel.ComputeCost(entry.Close, units);
      var netPnl = rawPnl - cost;

      results.Add(new TradeResult
      {
        EntryTime = entry.Time,
        ExitTime = exit.Time,
        RawPnL = rawPnl,
        NetPnL = netPnl,
        Cost = cost
      });
    }

    return results;
  }
}
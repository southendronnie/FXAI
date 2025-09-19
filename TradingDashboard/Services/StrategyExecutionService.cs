using TradingDashboard.Models;
using TradingDashboard.Models;

namespace TradingDashboard.Services
{
  public class StrategyExecutionService
  {
    private readonly OandaCostModel _costModel;

    public StrategyExecutionService(OandaCostModel costModel)
    {
      _costModel = costModel;
    }

    public TradeResult ExecuteStrategy(StrategyProfile profile, Candle entry, Candle exit)
    {
      var entryPrice = entry.Close;
      var exitPrice = exit.Close;
      var rawPnL = exitPrice - entryPrice;

      // Apply strategy-specific logic (e.g., reversal, momentum)
      if (profile.Parameters.TryGetValue("direction", out var direction) && direction == "short")
        rawPnL = entryPrice - exitPrice;

      var cost = _costModel.CalculateCost(entryPrice, exitPrice);

      return new TradeResult
      {
        StrategyId = profile.PatternType, // Fix for CS1061: Use an existing property from StrategyProfile
        EntryTime = entry.Time,
        ExitTime = exit.Time,
        EntryPrice = entryPrice,
        ExitPrice = exitPrice,
        ProfitLoss = rawPnL - cost
      };
    }

    public List<TradeResult> RunBatch(List<Candle> candles, StrategyProfile profile)
    {
      var results = new List<TradeResult>();
      for (int i = 0; i < candles.Count - 1; i++)
      {
        var entry = candles[i];
        var exit = candles[i + 1];
        var result = ExecuteStrategy(profile, entry, exit);
        results.Add(result);
      }
      return results;
    }
  }
}
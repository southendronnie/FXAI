using System.Collections.Generic;
using System.Threading.Tasks;

public class StrategyExecutionService
{
  private List<TradeResult> _latestTrades = new();

  public void ExecuteStrategy(
      StrategyProfile profile,
      List<Candle> candles,
      OandaCostModel costModel,
      int units)
  {
    _latestTrades = profile.Execute(candles, costModel, units);
  }

  public Task<List<TradeResult>> GetLatestTradesAsync()
  {
    return Task.FromResult(_latestTrades);
  }

  public void Reset()
  {
    _latestTrades.Clear();
  }
}
using System;
using System.Collections.Generic;
using System.Linq;

public class PatternStatsEngine
{
  private readonly Dictionary<string, List<decimal>> _rawPnls = new();
  private readonly Dictionary<string, decimal> _totalCosts = new();

  public void Record(string patternType, decimal rawPnl, decimal cost)
  {
    if (!_rawPnls.ContainsKey(patternType))
      _rawPnls[patternType] = new List<decimal>();

    _rawPnls[patternType].Add(rawPnl);

    if (!_totalCosts.ContainsKey(patternType))
      _totalCosts[patternType] = 0;

    _totalCosts[patternType] += cost;
  }

  public IEnumerable<PatternStats> GetStats()
  {
    foreach (var kvp in _rawPnls)
    {
      var pattern = kvp.Key;
      var returns = kvp.Value;
      var totalCost = _totalCosts.TryGetValue(pattern, out var cost) ? cost : 0;

      var count = returns.Count;
      var hitRate = count > 0 ? returns.Count(r => r > 0) / (double)count : 0;
      var avgRaw = count > 0 ? returns.Average() : 0;
      var avgCost = count > 0 ? totalCost / count : 0;
      var netPnl = avgRaw - avgCost;
      var maxDrawdown = count > 0 ? returns.Min() : 0;

      yield return new PatternStats
      {
        Type = pattern,
        Count = count,
        HitRate = hitRate,
        AveragePnL = avgRaw,
        TotalCost = avgCost,
        NetPnL = netPnl,
        MaxDrawdown = maxDrawdown
      };
    }
  }

  public void Reset()
  {
    _rawPnls.Clear();
    _totalCosts.Clear();
  }
}
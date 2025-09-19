using TradingDashboard.Models;

public class PatternService
{
  private readonly List<PatternStats> _statsBuffer = new();
  private readonly object _lock = new();

  public void AddStat(PatternStats stat)
  {
    lock (_lock)
    {
      _statsBuffer.Add(stat);
      if (_statsBuffer.Count > 500)
        _statsBuffer.RemoveAt(0); // keep buffer lean
    }
  }

  public List<PatternStats> GetLatestStats()
  {
    lock (_lock)
    {
      return _statsBuffer.ToList(); // shallow copy for thread safety
    }
  }

  public PatternStats? GetLatest()
  {
    lock (_lock)
    {
      return _statsBuffer.LastOrDefault();
    }
  }

  public void Clear()
  {
    lock (_lock)
    {
      _statsBuffer.Clear();
    }
  }

  public Dictionary<string, double> GetAggregates()
  {
    lock (_lock)
    {
      return new Dictionary<string, double>
      {
        ["WinRate"] = _statsBuffer.Average(s => s.WinRate),
        ["Drawdown"] = _statsBuffer.Average(s => s.MaxDrawdown),
        ["AvgReturn"] = _statsBuffer.Average(s => s.AverageReturn)
      };
    }
  }
}
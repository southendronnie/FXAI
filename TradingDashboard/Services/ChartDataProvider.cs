using TradingDashboard.Models;

public class ChartDataProvider
{
  private readonly PatternService _patternService;

  public ChartDataProvider(PatternService patternService)
  {
    _patternService = patternService;
  }

  public object BuildOptions(List<string> activeMetrics)
  {
    var stats = _patternService.GetLatestStats();

    var series = activeMetrics.Select(metric => new
    {
      name = metric,
      data = ExtractMetricSeries(stats, metric)
    }).ToList();

    return new
    {
      chart = new { type = "line" },
      title = new { text = "Pattern Metrics" },
      xAxis = new { categories = stats.Select(s => s.Timestamp.ToString("HH:mm")).ToArray() },
      yAxis = new { title = new { text = "Value" } },
      series
    };
  }

  private List<double> ExtractMetricSeries(List<PatternStats> stats, string metric)
  {
    return metric switch
    {
      "WinRate" => stats.Select(s => s.WinRate).ToList(),
      "Drawdown" => stats.Select(s => s.MaxDrawdown).ToList(),
      "AvgReturn" => stats.Select(s => s.AverageReturn).ToList(),
      _ => new List<double>()
    };
  }
}
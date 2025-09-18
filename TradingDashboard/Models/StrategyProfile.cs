namespace TradingDashboard.Models
{
  public class StrategyProfile
  {
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Strategy classification
    public string PatternType { get; set; } = "Unknown"; // e.g. "Breakout", "Reversal", "Trend"

    // Runtime parameters
    public Dictionary<string, string> Parameters { get; set; } = new();

    // Optional metadata
    public List<string> Tags { get; set; } = new();
    public string? Timeframe { get; set; }
    public string? Instrument { get; set; }

    // Execution flags
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastRun { get; set; }

    // Visual styling
    public string? Color { get; set; } // e.g. "#FF9900"
    public string? Icon { get; set; }  // e.g. "📈", "⚡", "🧠"

    // Typed parameter accessor
    public T GetParameter<T>(string key, T fallback)
    {
      if (Parameters.TryGetValue(key, out var value))
      {
        try
        {
          var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
          return (T)converter.ConvertFromInvariantString(value)!;
        }
        catch { }
      }
      return fallback;
    }

    // Optional validation
    public bool IsValid()
    {
      return !string.IsNullOrWhiteSpace(Name)
          && !string.IsNullOrWhiteSpace(PatternType)
          && Parameters.ContainsKey("direction")
          && (Parameters["direction"] == "long" || Parameters["direction"] == "short");
    }
  }
}
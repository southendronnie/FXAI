public class DiagnosticItem
{
  public string Name { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public string Status { get; set; } = "Unknown"; // Healthy, Warning, Critical
}
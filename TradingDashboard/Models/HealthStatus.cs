public class HealthStatus
{
  public bool IsHealthy { get; set; }
  public string Message { get; set; } = string.Empty;
  public DateTime CheckedAt { get; set; }
}
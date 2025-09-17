public class OrderRequest
{
  public string Side { get; set; } = "buy";
  public string Instrument { get; set; } = "EUR/USD";
  public int Quantity { get; set; } = 1000;
}
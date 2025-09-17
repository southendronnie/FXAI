public class OandaCostModel
{
  public decimal SpreadPips { get; set; }
  public decimal CommissionPerLot { get; set; }
  public int LotSize { get; set; } = 100_000;

  public decimal ComputeCost(decimal price, int units)
  {
    var pipValue = price * 0.0001m;
    var spreadCost = SpreadPips * pipValue * units / LotSize;
    var commissionCost = CommissionPerLot * units / LotSize;
    return spreadCost + commissionCost;
  }
}
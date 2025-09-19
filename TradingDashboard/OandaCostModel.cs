namespace TradingDashboard.Models
{
  public class OandaCostModel
  {
    public decimal Spread { get; set; } = 0.0002m; // Typical FX spread
    public decimal CommissionPerTrade { get; set; } = 0.0m; // Optional commission

    public decimal CalculateCost(decimal entryPrice, decimal exitPrice)
    {
      var slippage = Spread;
      var commission = CommissionPerTrade;
      return slippage + commission;
    }

    public decimal AdjustedPnL(decimal rawPnL)
    {
      return rawPnL - (Spread + CommissionPerTrade);
    }
  public decimal ComputeCost(decimal entryPrice, decimal exitPrice, string instrument)
    {
      var spread = GetSpreadForInstrument(instrument);
      var commission = GetCommissionForInstrument(instrument);
      return spread + commission;
    }

    private decimal GetSpreadForInstrument(string instrument)
    {
      return instrument switch
      {
        "EURUSD" => 0.0002m,
        "GBPJPY" => 0.03m,
        _ => Spread
      };
    }

    private decimal GetCommissionForInstrument(string instrument)
    {
      return instrument switch
      {
        "EURUSD" => 0.0m,
        "XAUUSD" => 0.5m,
        _ => CommissionPerTrade
      };
    }
    public decimal ComputeCost(decimal entryPrice, decimal exitPrice)
    {
      var slippage = Spread;
      var commission = CommissionPerTrade;
      return slippage + commission;
    }
  }
  }
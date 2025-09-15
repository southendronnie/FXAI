using Microsoft.AspNetCore.Hosting.Server;
using System.Net;
using static OandaDataApi.Statics;

namespace OandaDataApi
{
  public enum EServer
  {
    Account,
    Labs,
    PricingStream,
    TransactionsStream
  }

  public class Credentials
  {
    public bool HasServer(EServer server)
    {
      return Servers[Environment].ContainsKey(server);
    }

    public string GetServer(EServer server)
    {
      if (HasServer(server))
      {
        return Servers[Environment][server];
      }
      return null;
    }

    private static readonly Dictionary<EEnvironment, Dictionary<EServer, string>> Servers = new Dictionary<EEnvironment, Dictionary<EServer, string>>
      {
         {  EEnvironment.Practice, new Dictionary<EServer, string>
            {
               {EServer.Account, "https://api-fxpractice.oanda.com/v3/"},
               {EServer.Labs, "https://api-fxpractice.oanda.com/labs/v3/"},
               {EServer.PricingStream, "https://stream-fxpractice.oanda.com/v3/"},
               {EServer.TransactionsStream, "https://stream-fxpractice.oanda.com/v3/"},
            }
         },
         {  EEnvironment.Trade, new Dictionary<EServer, string>
            {
               {EServer.Account, "https://api-fxtrade.oanda.com/v3/"},
               {EServer.Labs, "https://api-fxtrade.oanda.com/labs/v3/"},
               {EServer.PricingStream, "https://stream-fxtrade.oanda.com/v3/"},
               {EServer.TransactionsStream, "https://stream-fxtrade.oanda.com/v3/"}
            }
         }
      };

    internal static Credentials GetDefaultCredentialsToken()
    {
      return m_DefaultCredentialsLong;
    }

    private static Credentials m_DefaultCredentialsLong;
    private static Credentials m_DefaultCredentialsShort;

    public string AccessToken { get; set; }
    public string DefaultAccountId { get; set; }
    public EEnvironment Environment { get; set; }

    public static Credentials GetDefaultCredentials(bool isLong)
    {
      return isLong ? m_DefaultCredentialsLong : m_DefaultCredentialsShort;
    }

    public static void SetCredentials(EEnvironment environment, string accessToken, string defaultAccount = "0")
    {
      m_DefaultCredentialsLong = new Credentials
      {
        Environment = environment,
        AccessToken = accessToken,
        DefaultAccountId = defaultAccount
      };
    }

    public static void SetCredentialsShort(EEnvironment environment, string accessToken, string defaultAccount = "0")
    {
      m_DefaultCredentialsShort = new Credentials
      {
        Environment = environment,
        AccessToken = accessToken,
        DefaultAccountId = defaultAccount
      };
    }
  }
  public class Statics
  {
    public enum EEnvironment
    {
      Practice,
      Trade
    }

    static EEnvironment m_TestEnvironment = EEnvironment.Practice;
    static string m_TestToken = "4a93aad4389861017e80406f6f4ec65c-511c0fa8718ca44edbeec8652571dd2a";
    static string m_TestAccount = "101-004-8806632-006";
    public static async Task Login()
    {
      //_sampleParams = new SampleParams(ConfigurationManager.AppSettings);
      //_fxcmsession = new Session(_sampleParams.AccessToken, _sampleParams.Url);
      //Console.WriteLine("Session status: Connecting");
      //_fxcmsession.Connect();


      Console.BackgroundColor = ConsoleColor.Black;
      Credentials.SetCredentials(m_TestEnvironment, m_TestToken, m_TestAccount);
    }
  }
}

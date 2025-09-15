using System.Net.Http.Headers;
using System.Text.Json;
using MemoryPack;
using Microsoft.AspNetCore.OpenApi; // Add this using directive at the top of your file

DateTime lastTickTime = DateTime.MinValue;
var startTime = DateTime.UtcNow;
PriceTick? latestTick = null;
Candle? latest1m = null;
Candle? latest5m = null;

// --- App Setup ---
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var accountId = builder.Configuration["Oanda:AccountId"];
var token = builder.Configuration["Oanda:AccessToken"];
var isPractice = bool.Parse(builder.Configuration["Oanda:IsPractice"] ?? "true");

var basePath = Path.Combine(AppContext.BaseDirectory, "CandleData");
Directory.CreateDirectory(basePath);
Directory.CreateDirectory(Path.Combine(basePath, "Ticks"));

// --- Capture Helpers ---
void AppendMemoryPack<T>(string filePath, T item)
{
  List<T> list;
  if (File.Exists(filePath))
  {
    var bytes = File.ReadAllBytes(filePath);
    list = MemoryPackSerializer.Deserialize<List<T>>(bytes) ?? new List<T>();
  }
  else list = new List<T>();

  list.Add(item);
  var newBytes = MemoryPackSerializer.Serialize(list);
  File.WriteAllBytes(filePath, newBytes);
}

void AppendCandle(string timeframe, Candle candle)
{
  var file = Path.Combine(basePath, $"{timeframe}_{candle.Time:yyyy-MM-dd}.bin");
  AppendMemoryPack(file, candle);
}

void AppendTick(PriceTick tick)
{
  var file = Path.Combine(basePath, "Ticks", $"ticks_{tick.Time:yyyy-MM-dd}.bin");
  AppendMemoryPack(file, tick);
}

// --- Candle Builders ---
Candle? current1m = null, current5m = null;
DateTime bucket1m = DateTime.MinValue, bucket5m = DateTime.MinValue;

void ProcessTick(DateTime time, decimal bid, decimal ask)
{
  lastTickTime = time;
  latestTick = new PriceTick { Time = time, Bid = bid, Ask = ask };

  var mid = (bid + ask) / 2;
  AppendTick(latestTick);

  var b1 = new DateTime(time.Ticks - (time.Ticks % TimeSpan.FromMinutes(1).Ticks), time.Kind);
  if (current1m == null || b1 != bucket1m)
  {
    if (current1m != null) AppendCandle("1m", current1m);
    bucket1m = b1;
    current1m = new Candle { Time = b1, Open = mid, High = mid, Low = mid, Close = mid };
  }
  else
  {
    current1m.High = Math.Max(current1m.High, mid);
    current1m.Low = Math.Min(current1m.Low, mid);
    current1m.Close = mid;
  }
  latest1m = current1m;

  var b5 = new DateTime(time.Ticks - (time.Ticks % TimeSpan.FromMinutes(5).Ticks), time.Kind);
  if (current5m == null || b5 != bucket5m)
  {
    if (current5m != null) AppendCandle("5m", current5m);
    bucket5m = b5;
    current5m = new Candle { Time = b5, Open = mid, High = mid, Low = mid, Close = mid };
  }
  else
  {
    current5m.High = Math.Max(current5m.High, mid);
    current5m.Low = Math.Min(current5m.Low, mid);
    current5m.Close = mid;
  }
  latest5m = current5m;
}

// Add a helper to parse the OANDA JSON structure with "prices" array
void ParseOandaPriceJson(string json)
{
    using var doc = JsonDocument.Parse(json);
    if (doc.RootElement.TryGetProperty("prices", out var pricesArr))
    {
        foreach (var priceObj in pricesArr.EnumerateArray())
        {
            if (priceObj.TryGetProperty("type", out var typeProp) &&
                typeProp.GetString() == "PRICE")
            {
                var time = priceObj.GetProperty("time").GetDateTime();
                var bid = Decimal.Parse (priceObj.GetProperty("bids")[0].GetProperty("price").ToString());
                var ask = Decimal.Parse (priceObj.GetProperty("asks")[0].GetProperty("price").ToString());
                ProcessTick(time, bid, ask);
            }
        }
    }
}
async Task PollOandaPrices(string instrument, CancellationToken ct = default)
{
  var domain = isPractice ? "api-fxpractice.oanda.com" : "api-fxtrade.oanda.com";
  var url = $"https://{domain}/v3/accounts/{accountId}/pricing?instruments={instrument}";

  using var http = new HttpClient();
  http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

  while (!ct.IsCancellationRequested)
  {
    try
    {
      var json = await http.GetStringAsync(url, ct);
      ParseOandaPriceJson(json); // already defined in your code
    }
    catch
    {
      // Optional: log or retry
    }

    await Task.Delay(TimeSpan.FromSeconds(2), ct); // Adjust polling interval as needed
  }
}
// --- OANDA Streaming ---
async Task StartOandaStream(string instrument, CancellationToken ct = default)
{//https://api-fxpractice.oanda.com/v3/accounts/101-004-8806632-007/summary

  var domain = isPractice ? "api-fxpractice.oanda.com" : "api-fxtrade.oanda.com";
  var url = $"https://{domain}/v3/accounts/{accountId}/pricing?instruments=GBP_USD";
  using var http = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
 
  
  http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

  using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
  response.EnsureSuccessStatusCode();

  await using var stream = await response.Content.ReadAsStreamAsync(ct);
  using var reader = new StreamReader(stream);

  while (!reader.EndOfStream && !ct.IsCancellationRequested)
  {
    var line = await reader.ReadLineAsync();
    if (string.IsNullOrWhiteSpace(line)) continue;

    try
    {
        using var doc = JsonDocument.Parse(line);
        // Handle "prices" array (OANDA batch format)
        if (doc.RootElement.TryGetProperty("prices", out var pricesArr))
        {
            foreach (var priceObj in pricesArr.EnumerateArray())
            {
                if (priceObj.TryGetProperty("type", out var typeProp) &&
                    typeProp.GetString() == "PRICE")
                {
                    var time = priceObj.GetProperty("time").GetDateTime();
                    var bid = priceObj.GetProperty("bids")[0].GetProperty("price").GetDecimal();
                    var ask = priceObj.GetProperty("asks")[0].GetProperty("price").GetDecimal();
                    ProcessTick(time, bid, ask);
                }
            }
        }
        // Handle single price object (OANDA single format)
        else if (doc.RootElement.TryGetProperty("type", out var typeProp) &&
                 typeProp.GetString() == "PRICE")
        {
            var time = doc.RootElement.GetProperty("time").GetDateTime();
            var bid = doc.RootElement.GetProperty("bids")[0].GetProperty("price").GetDecimal();
            var ask = doc.RootElement.GetProperty("asks")[0].GetProperty("price").GetDecimal();
            ProcessTick(time, bid, ask);
        }
    }
    catch
    {
        // Ignore malformed lines
    }
  }
}

_ = Task.Run(() => PollOandaPrices("EUR_USD"));

// --- API Endpoints ---
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
  options.SwaggerEndpoint("/swagger/v1/swagger.json", "OANDA Data API v1");
  options.RoutePrefix = "";
});

app.MapGet("/api/candles/{timeframe}", (string timeframe, DateTime start, DateTime end) =>
{
  var all = new List<Candle>();
  for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
  {
    var file = Path.Combine(basePath, $"{timeframe}_{date:yyyy-MM-dd}.bin");
    if (!File.Exists(file)) continue;
    var bytes = File.ReadAllBytes(file);
    var list = MemoryPackSerializer.Deserialize<List<Candle>>(bytes);
    if (list != null) all.AddRange(list.Where(c => c.Time >= start && c.Time <= end));
  }
  return Results.Ok(all);
})
.WithName("GetCandles")
.WithTags("Candles").WithOpenApi();

app.MapGet("/api/candles/{timeframe}/mp", (string timeframe, DateTime start, DateTime end) =>
{
  var all = new List<Candle>();
  for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
  {
    var file = Path.Combine(basePath, $"{timeframe}_{date:yyyy-MM-dd}.bin");
    if (!File.Exists(file)) continue;
    var bytes = File.ReadAllBytes(file);
    var list = MemoryPackSerializer.Deserialize<List<Candle>>(bytes);
    if (list != null) all.AddRange(list.Where(c => c.Time >= start && c.Time <= end));
  }
  var payload = MemoryPackSerializer.Serialize(all);
  return Results.File(payload, "application/octet-stream");
})
.WithName("GetCandlesMemoryPack")
.WithTags("Candles")
.WithOpenApi();

app.MapGet("/api/ticks", (DateTime start, DateTime end) =>
{
  var all = new List<PriceTick>();
  var tickPath = Path.Combine(basePath, "Ticks");
  for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
  {
    var file = Path.Combine(tickPath, $"ticks_{date:yyyy-MM-dd}.bin");
    if (!File.Exists(file)) continue;
    var bytes = File.ReadAllBytes(file);
    var list = MemoryPackSerializer.Deserialize<List<PriceTick>>(bytes);
    if (list != null) all.AddRange(list.Where(t => t.Time >= start && t.Time <= end));
  }
  return Results.Ok(all);
})
.WithName("GetTicks")
.WithTags("Ticks")
.WithOpenApi();

app.MapGet("/api/ticks/mp", (DateTime start, DateTime end) =>
{
  var all = new List<PriceTick>();
  var tickPath = Path.Combine(basePath, "Ticks");
  for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
  {
    var file = Path.Combine(tickPath, $"ticks_{date:yyyy-MM-dd}.bin");
    if (!File.Exists(file)) continue;
    var bytes = File.ReadAllBytes(file);
    var list = MemoryPackSerializer.Deserialize<List<PriceTick>>(bytes);
    if (list != null) all.AddRange(list.Where(t => t.Time >= start && t.Time <= end));
  }
  var payload = MemoryPackSerializer.Serialize(all);
  return Results.File(payload, "application/octet-stream");
})
.WithName("GetTicksMemoryPack")
.WithTags("Ticks")
.WithOpenApi();

app.MapGet("/health", () =>
{
  var now = DateTime.UtcNow;
  var uptime = now - startTime;
  var secondsSinceLastTick = lastTickTime == DateTime.MinValue
      ? (int?)null
      : (int)(now - lastTickTime).TotalSeconds;

  return Results.Ok(new
  {
    status = secondsSinceLastTick.HasValue && secondsSinceLastTick < 30 ? "ok" : "stale",
    uptime = $"{(int)uptime.TotalMinutes}m",
    lastTick = latestTick,
    latest1m,
    latest5m
  });
})
.WithName("HealthCheck")
.WithTags("Diagnostics")
.WithOpenApi();

app.MapGet("/stream/status", () =>
{
  return Results.Ok(new
  {
    lastTickTime,
    latestTick,
    latest1m,
    latest5m
  });
})
.WithName("StreamStatus")
.WithTags("Diagnostics")
.WithOpenApi();

app.Run();

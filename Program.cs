using MemoryPack;
using System.Net.Http.Headers;
using System.Text.Json;



// --- App Setup ---
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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
    var mid = (bid + ask) / 2;
    AppendTick(new PriceTick { Time = time, Bid = bid, Ask = ask });

    // 1m candle
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

    // 5m candle
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
}

// --- OANDA Streaming ---
async Task StartOandaStream(string instrument, CancellationToken ct = default)
{
    var domain = isPractice ? "stream-fxpractice.oanda.com" : "stream-fxtrade.oanda.com";
    var url = $"https://{domain}/v3/accounts/{accountId}/pricing/stream?instruments={instrument}";

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
            if (doc.RootElement.TryGetProperty("type", out var typeProp) &&
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
            // Ignore malformed lines (e.g., heartbeats)
        }
    }
}

_ = Task.Run(() => StartOandaStream("EUR_USD"));

// --- API Endpoints ---
var app = builder.Build();

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
});

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
});

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
});

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
});

app.Run();

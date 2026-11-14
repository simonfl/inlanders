using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Inlanders.Browser;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = args, ContentRootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../..")) });
builder.WebHost.UseUrls("http://127.0.0.1:5180");
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 32 * 1024);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
builder.Services.AddResponseCompression(o => { o.EnableForHttps = true; o.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/x-ndjson" }); });
builder.Services.AddSingleton<VillageSessions>();
builder.Services.AddHostedService<ClockService>();
var app = builder.Build();
app.UseResponseCompression();
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Content-Security-Policy"] = "frame-ancestors 'none'";
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        context.Response.Headers.CacheControl = "no-store";
        // Loopback-only listener + private Tailscale Serve. Reject cross-site browser writes.
        var origin = context.Request.Headers.Origin.ToString();
        if (origin.Length > 0 && (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.Authority != context.Request.Host.Value))
        { context.Response.StatusCode = 403; return; }
    }
    try { await next(); }
    catch (KeyNotFoundException) { context.Response.StatusCode = 404; await context.Response.WriteAsJsonAsync(new { error = "This village could not be found. Start a new village." }); }
    catch (ArgumentException e) { context.Response.StatusCode = 400; await context.Response.WriteAsJsonAsync(new { error = e.Message }); }
});
app.MapGet("/api/health", () => new { status = "ready", build = VillageSessions.Build });
app.MapPost("/api/sessions", (string? scenario, VillageSessions villages) => villages.Create(scenario ?? "crossing"));
app.MapGet("/api/sessions/{id}", (string id, VillageSessions villages) => villages.Observe(id));
app.MapPost("/api/sessions/{id}/command", (string id, Order order, VillageSessions villages) => villages.Command(id, order));
app.MapPost("/api/sessions/{id}/preview", (string id, Order order, VillageSessions villages) => villages.Preview(id, order));
app.MapPost("/api/sessions/{id}/heartbeat", (string id, ClientView view, VillageSessions villages) => villages.Heartbeat(id, view));
app.MapGet("/api/sessions/{id}/recording", (string id, VillageSessions villages) => Results.Text(villages.Recording(id), "application/x-ndjson"));
app.MapGet("/api/guide", () => new {
    title = "Les Habitants browser experiments", premise = "POST /api/sessions?scenario=room-by-river for an established four-person village expecting four more neighbors. Keep useful nearby woodland and use the open far bank, or clear nearby ground. The old crossing works; an optional six-log shortcut reduces walking. room-by-river-far starts the working garden on the far bank for comparison. Arrivals after 180s pause; welcome early when ready. No final sustain timer. Wooded home/food placements automatically clear then build; previews disclose that consequence. POST /api/sessions?scenario=timber-trade for an optional twelve-log/sixteen-food trade arriving after 45 simulation seconds, before four neighbors arrive, timber-trade-ripe for a nearly ripe first crop, or timber-control for the same slow-crop setup without a trader. Trader arrival, departure and neighbor arrivals pause; local-food completion has no sustain timer. POST /api/sessions?scenario=crossing for the short recovery, or ?scenario=river-closing for a longer growing settlement. The latter starts with a working bridge and a growing field: prepare homes and food for two arrivals, then sustain twelve neighbors after closure. Arrivals and closure pause automatically. Hunger is recoverable.",
    instructions = "Requests use JSON with a type field; see example below. POST /api/sessions/{id}/preview to preview a placement; POST /api/sessions/{id}/command to execute; GET /api/sessions/{id} to observe. Create a session, then observe its visible world and objectives. Start paused. Preview actions before confirming. Commands share the human game's rules. Advance at most 30 seconds while paused; untilEvent stops at a changed goal or event. No hidden state is provided.",
    example = new { type = "home", x = 1, z = 6, rotation = 0 },
    commands = new[] { "garden-entrance (Room by the river only; siteId of completed vegetable garden; preview opposite entrance on same ground and actual food-store walk; crop retained, no timber cost)", "shortcut (Room by the river only; authored optional six-log crossing; preview gives route comparison)", "welcome (Room by the river only; bring expected neighbors early when visible readiness allows)", "dock (x,z,rotation; 12 logs; shore with dry entrance and water launch; settlement scenarios only; automatic fishing, shared renewing grounds)", "trade-buy (timber scenarios: agree to pack 12 logs automatically for 16 food; accepts zero available logs; provisions only after full shipment; accepted orders have no deadline)", "trade-decline (keep timber and dismiss offer)", "trade-cancel (cancel an unfinished shipment and return packed logs)", "repair (short crossing only)", "garden (x,z,rotation; 4 logs)", "home (x,z,rotation; 6 logs, two beds; river only)", "field (x,z,rotation; 10 logs, larger harvest; river only)", "grove (x,z,x2,z2)", "next-season (river only; gated by visible readiness, brings next event early)", "cancel (siteId; unfinished plans only)", "pause", "resume (speed:1,3,6)", "advance (seconds:1..30, untilEvent:true|false)", "bookmark (note)", "view (view: camera/selection/panel/viewport)" }
});
var client = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "../client/dist"));
if (Directory.Exists(client))
{
    var files = new PhysicalFileProvider(client);
    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = files });
    app.UseStaticFiles(new StaticFileOptions { FileProvider = files });
}
app.Run();

namespace Inlanders.Browser
{
    sealed class ClockService(VillageSessions sessions) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken token)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
            try { while (await timer.WaitForNextTickAsync(token)) sessions.Tick(); }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        }
        public override Task StopAsync(CancellationToken cancellationToken) { sessions.SaveAll(); return base.StopAsync(cancellationToken); }
    }
}

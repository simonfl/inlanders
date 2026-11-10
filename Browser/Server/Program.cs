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
app.MapPost("/api/sessions", (VillageSessions villages) => villages.Create());
app.MapGet("/api/sessions/{id}", (string id, VillageSessions villages) => villages.Observe(id));
app.MapPost("/api/sessions/{id}/command", (string id, Order order, VillageSessions villages) => villages.Command(id, order));
app.MapPost("/api/sessions/{id}/preview", (string id, Order order, VillageSessions villages) => villages.Preview(id, order));
app.MapPost("/api/sessions/{id}/heartbeat", (string id, ClientView view, VillageSessions villages) => villages.Heartbeat(id, view));
app.MapGet("/api/sessions/{id}/recording", (string id, VillageSessions villages) => Results.Text(villages.Recording(id), "application/x-ndjson"));
app.MapGet("/api/guide", () => new {
    title = "The broken crossing", premise = "Four neighbors have safe homes but their harvest is across a washed-out crossing. Restore access or grow food nearby.",
    instructions = "Create a session, then observe its visible world and objectives. Start paused. Preview actions before confirming. Commands share the human game's rules. Advance at most 30 seconds while paused; untilEvent stops at a changed goal or event. No hidden state is provided.",
    commands = new[] { "repair", "garden (x,z,rotation)", "grove (x,z,x2,z2)", "cancel (siteId; unfinished plans only)", "pause", "resume (speed:1,3,6)", "advance (seconds:1..30, untilEvent:true|false)", "bookmark (note)", "view (view: camera/selection/panel/viewport)" }
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

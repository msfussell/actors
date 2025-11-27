using Light.Service;

var builder = WebApplication.CreateBuilder(args);
// Ensure WebHost binds to the port Dapr expects
builder.WebHost.UseUrls("http://0.0.0.0:5008");

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<LightActor>();
    // Optional runtime tuning:
    // options.ActorIdleTimeout = TimeSpan.FromMinutes(5);
    // options.ActorScanInterval = TimeSpan.FromSeconds(30);
    // options.DrainOngoingCallTimeout = TimeSpan.FromSeconds(30);
    // options.DrainRebalancedActors = true;
});

var app = builder.Build();

// Add logging middleware
app.UseRouting();

app.MapActorsHandlers();
app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("ActorService is up and running");
    return "ActorService is up and running. You can now create LightActors using a client.";
});

app.Run();


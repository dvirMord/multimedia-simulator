using video_simulator.constans;
using video_simulator.MediaMTX;
using video_simulator.Services;
using VideoSimulator.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
//DI 
builder.Services.AddSingleton<IVideoSimulatorService, VideoSimulatorService>();
builder.Services.AddSingleton<IStorageService, LocalStorageService>();
builder.Services.AddSingleton<IFFmpegManager, FFmpegManager>();

//create mediaMtx server and start it
var app = builder.Build();

var mediaMtxPath = Environment.GetEnvironmentVariable(EnvConstants.mediaMTXPathName)
                    ?? throw new InvalidOperationException("MediaMTX path is not configured.");
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var ffmpegManager = app.Services.GetRequiredService<IFFmpegManager>();

MediaMtxServer mediaMtxServer = new MediaMtxServer(mediaMtxPath, app.Services.GetRequiredService<ILogger<MediaMtxServer>>());
await mediaMtxServer.StartAsync(rtspPort: 8554);


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

//Register a callback when the stopping signal is received
lifetime.ApplicationStopping.Register(() =>
{
    logger.LogInformation("Graceful shutdown initiated (ApplicationStopping)...");
    mediaMtxServer.Stop();
    ffmpegManager.KillAllRunningStreams();
});

app.Run();

using video_simulator.constans;
using video_simulator.MediaMTX;
using video_simulator.Services;
using video_simulator.Interfaces;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

//DI 
builder.Services.AddSingleton<IVideoSimulatorService, VideoSimulatorService>();
builder.Services.AddSingleton<IStorageService, LocalStorageService>();
builder.Services.AddSingleton<IFFmpegManager, FFmpegManager>();

var app = builder.Build();

//create mediaMtx server and start it
var mediaMtxPath = Environment.GetEnvironmentVariable(EnvConstants.mediaMTXPathName)
                    ?? throw new InvalidOperationException(MediaMtxExceptions.MediaMTXPathNotConfigured);

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var ffmpegManager = app.Services.GetRequiredService<IFFmpegManager>();

MediaMtxServer mediaMtxServer = new MediaMtxServer(mediaMtxPath, app.Services.GetRequiredService<ILogger<MediaMtxServer>>());
await mediaMtxServer.StartAsync(rtspPort: Constants.RTSP_DEFAULT_PORT);


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

Console.Write("Press any key to exit...");
Console.Read();

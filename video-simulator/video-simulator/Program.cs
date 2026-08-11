using video_simulator.Services;
using video_simulator.constans;
using VideoSimulator.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
//DI 
builder.Services.AddScoped<IVideoSimulatorService, VideoSimulatorService>();
builder.Services.AddScoped<IStorageService, LocalStorageService>();
//singalton
FfmpegManager ffmpegManager = await FfmpegManager.CreateAsnc();
builder.Services.AddSingleton<IFfmpegManager>(ffmpegManager);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

// Register a callback when the stopping signal is received
lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Graceful shutdown initiated (ApplicationStopping)...");
    ffmpegManager.cleanUpPorcesss();
});

app.Run();

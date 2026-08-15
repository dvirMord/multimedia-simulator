using multimedia_simulator.constans;
using multimedia_simulator.MediaMTX;
using multimedia_simulator.Services;
using multimedia_simulator.Interfaces;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

string dbPath = Environment.GetEnvironmentVariable(EnvConstants.dbConnectionStringName)
                ?? throw new InvalidOperationException(DBManagerExceptions.DBPathNotConfigured);

//---------------DIs------------------------------------------------------------
builder.Services.AddSingleton<IMultimediaFilesService, MultimediaFilesService>();
builder.Services.AddSingleton<IRtspStreamsService, RtspStreamsService>();
builder.Services.AddSingleton<IStorageService, LocalStorageService>();
builder.Services.AddSingleton<IFFmpegManager, FFmpegManager>();
builder.Services.AddSingleton<IDBManager>(db => new SQLiteManager(dbPath));
//---------------------------------------------------------------------------------

var app = builder.Build();


//-----------------get required services---------------------------------------------------------
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var ffmpegManager = app.Services.GetRequiredService<IFFmpegManager>();
var dbManager = app.Services.GetRequiredService<IDBManager>();
//--------------------------------------------------------------------------------

//-------------start mediaMtx server---------------------------------------------------------
var mediaMtxPath = Environment.GetEnvironmentVariable(EnvConstants.mediaMTXPathName)
                    ?? throw new InvalidOperationException(MediaMtxExceptions.MediaMTXPathNotConfigured);

MediaMtxServer mediaMtxServer = new MediaMtxServer(mediaMtxPath, app.Services.GetRequiredService<ILogger<MediaMtxServer>>());
await mediaMtxServer.StartAsync(rtspPort: Constants.RTSP_DEFAULT_PORT);
//--------------------------------------------------------------------------------------------------------------

//-----------------Initialize database if not exists---------------------------------------------------------
logger.LogInformation("Initializing SQLite database....");
await dbManager.InitializeDatabaseAsync();
logger.LogInformation("Database initialized successfully.");
//--------------------------------------------------------------------------------------------------------------

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

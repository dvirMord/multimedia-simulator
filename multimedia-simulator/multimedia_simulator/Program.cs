using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using multimedia_simulator.constants;
using multimedia_simulator.Interfaces;
using multimedia_simulator.MediaMTX;
using multimedia_simulator.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//--------------- API Versioning Configuration ---------------------------------
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddOpenApi("v1");
//------------------------------------------------------------------------------    

//--------------- DIs ---------------------------------------------------------
string dbPath = Environment.GetEnvironmentVariable(EnvConstants.dbConnectionStringName)
                ?? throw new InvalidOperationException(DBManagerExceptions.DBPathNotConfigured);

builder.Services.AddSingleton<IMultimediaFilesService, MultimediaFilesService>();
builder.Services.AddSingleton<IRtspStreamsService, RtspStreamsService>();
builder.Services.AddSingleton<IStorageService, LocalStorageService>();
builder.Services.AddSingleton<IFFmpegManager, FFmpegManager>();
builder.Services.AddSingleton<IDBManager>(db => new SQLiteManager(dbPath));

var app = builder.Build();

//----------------- Get required services -------------------------------------
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var ffmpegManager = app.Services.GetRequiredService<IFFmpegManager>();
var dbManager = app.Services.GetRequiredService<IDBManager>();

//------------- Start mediaMtx server -----------------------------------------
var mediaMtxPath = Environment.GetEnvironmentVariable(EnvConstants.mediaMTXPathName)
                    ?? throw new InvalidOperationException(MediaMtxExceptions.MediaMTXPathNotConfigured);

MediaMtxServer mediaMtxServer = new MediaMtxServer(mediaMtxPath, app.Services.GetRequiredService<ILogger<MediaMtxServer>>());
await mediaMtxServer.StartAsync(rtspPort: Constants.RTSP_DEFAULT_PORT);

//----------------- Initialize database if not exists -------------------------
logger.LogInformation("Initializing SQLite database....");
await dbManager.InitializeDatabaseAsync();
logger.LogInformation("Database initialized successfully.");

//----------------- Swagger UI Setup ------------------------------------------
if (app.Environment.IsDevelopment())
{
    // חושף את ה-JSON בנתיב /openapi/{documentName}.json
    app.MapOpenApi();

    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwaggerUI(options =>
    {
        // ממפה את כל הגרסאות המוגדרות ב-API Versioning ישירות ל-Dropdown של Swagger UI
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/openapi/{description.GroupName}.json",
                $"API {description.GroupName.ToUpperInvariant()}"
            );
        }

        // מגדיר את נתיב הגישה ל-Swagger
        options.RoutePrefix = "swagger";
    });
}

app.UseAuthorization();
app.MapControllers();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStopping.Register(() =>
{
    logger.LogInformation("Graceful shutdown initiated (ApplicationStopping)...");
    mediaMtxServer.Stop();
    ffmpegManager.KillAllRunningStreams();
});

app.Run();

Console.Write("Press any key to exit...");
Console.Read();
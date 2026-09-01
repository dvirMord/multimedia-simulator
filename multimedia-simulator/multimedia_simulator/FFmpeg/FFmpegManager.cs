using System.Collections.Concurrent;
using System.Diagnostics;
using multimedia_simulator.Interfaces;
using multimedia_simulator.constants;
using multimedia_simulator.Validators;

namespace multimedia_simulator.Services
{
    public class FFmpegManager : IFFmpegManager
    {
        private readonly ConcurrentDictionary<string, Process> _runningStreams;
        private readonly string _ffmpegPath;
        private readonly string _storagePath;
        private readonly ILogger<FFmpegManager> _logger;


        //-----------------constructor-----------------
        public FFmpegManager(ILogger<FFmpegManager> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._runningStreams = new ConcurrentDictionary<string, Process>();

            this._ffmpegPath =
                Environment.GetEnvironmentVariable(EnvConstants.ffmpegPathName)
                ?? throw new InvalidOperationException(FFmpegExceptions.FFmpegPathNotConfigured);

            this._storagePath = Environment.GetEnvironmentVariable(EnvConstants.tsFilesStorageName)
                ?? throw new InvalidOperationException(FFmpegExceptions.TsFilesStoragePathNotConfigured);
        }

        //-----------------interface functions-----------------

        public async Task<int> StartStreamAsync(string streamId)
        {
            MyValidators.ValidateNotNullOrEmpty(streamId);
            MyValidators.ValidateFileExtension(streamId);

            if (this.IsStreamRunning(streamId))
            {
                throw new ArgumentException(string.Format(FFmpegExceptions.StreamAlreadyRunningTemplate, streamId));
            }

            MyValidators.ValidateFileExists(streamId, this._storagePath);

            string fullPath = Path.Combine(this._storagePath, streamId);

            string args =
                 $"-re " +                                      // Read input at native frame rate
                 $"-stream_loop -1 " +                          // Infinite loop
                 $"-i \"{fullPath}\" " +
                 $"-map 0:v:0 " +
                 $"-c:v copy " +
                 $"-f rtsp " +
                 $"-rtsp_transport tcp " +                      // FORCE TCP to prevent dropped packets
                 $"-muxdelay 0.1 " +                            // Reduce stream latency
                 $"rtsp://localhost:8554/{streamId}";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = this._ffmpegPath,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };

            try
            {
                Process process = new Process
                {
                    StartInfo = startInfo,
                    EnableRaisingEvents = true
                };

                process.Start();
                process.BeginErrorReadLine();

                await Task.Delay(Constants.FFMPEG_STARTUP_CHECK_MILLISECONDS);

                if (process.HasExited)
                {
                    string errorLog = await process.StandardError.ReadToEndAsync();

                    _logger.LogError(FFmpegManagerMessages.Error.ProcessCrashedImmediately);
                    _logger.LogError(FFmpegManagerMessages.Error.FFmpegErrorOutputTemplate, errorLog);

                    process.Dispose();

                    throw new InvalidOperationException(FFmpegManagerMessages.Error.ProcessCrashedImmediately);
                }

                _logger.LogInformation(FFmpegManagerMessages.Success.StreamRunningTemplate, streamId);

                this._runningStreams.TryAdd(streamId, process);
                return process.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, FFmpegManagerMessages.Error.FailedToStartStreamTemplate, ex.Message);

                throw new InvalidOperationException(string.Format(FFmpegExceptions.StreamStartFailedTemplate, streamId, ex.Message), ex);
            }
        }

        public async Task StopStreamAsync(string streamId)
        {
            MyValidators.ValidateNotNullOrEmpty(streamId);

            if (!this._runningStreams.TryGetValue(streamId, out Process? process))
            {
                throw new ArgumentException(string.Format(FFmpegExceptions.StreamNotRunningTemplate, streamId));
            }

            _logger.LogInformation(FFmpegManagerMessages.Success.StreamStoppedTemplate, streamId);

            if (!process.HasExited)
            {
                process.Kill(true);
                await process.WaitForExitAsync();
            }

            this._runningStreams.TryRemove(streamId, out _);

            process.Dispose();
        }

        public bool IsStreamRunning(string streamId)
        {
            MyValidators.ValidateNotNullOrEmpty(streamId);

            return this._runningStreams.ContainsKey(streamId);
        }

        public List<string> GetRunningStreams()
        {
            return this._runningStreams.Keys.ToList();
        }

        //-----------------create and clean-------------
        public async Task KillAllRunningStreams()
        {
            foreach (string streamId in this._runningStreams.Keys.ToList())
            {
                await this.StopStreamAsync(streamId);
            }
        }
    }
}
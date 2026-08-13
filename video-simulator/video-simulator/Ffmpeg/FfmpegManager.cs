using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using video_simulator.constans;
using video_simulator.MediaMTX;
using video_simulator.Validators;

namespace video_simulator.Services
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
                ?? throw new InvalidOperationException("FFmpeg path is not configured.");

            this._storagePath = Environment.GetEnvironmentVariable(EnvConstants.tsFilesStorageName)
                ?? throw new InvalidOperationException("TS files storage path is not configured.");
        }

        //-----------------interface functions-----------------

        public bool StartStream(string streamId)
        {
            MyValidators.ValidateNotNullOrEmpty(streamId);
            MyValidators.ValidateFileExtension(streamId);

            if (this.IsStreamRunning(streamId))
            {
                throw new ArgumentException($"{streamId} Stream is already running");
            }

            MyValidators.ValidateFileExists(
                streamId,
                this._storagePath);

            string fullPath = Path.Combine(
                this._storagePath,
                streamId);

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
                    StartInfo = startInfo
                };

                process.Start();

                Thread.Sleep(500);

                if (process.HasExited)
                {
                    string errorLog =
                        process.StandardError.ReadToEnd();

                    _logger.LogError(FFmpegManagerMessages.Error.ProcessCrashedImmediately);
                    _logger.LogError(FFmpegManagerMessages.Error.FFmpegErrorOutputTemplate, errorLog);

                    process.Dispose();

                    return false;
                }

                _logger.LogInformation(FFmpegManagerMessages.Success.StreamRunningTemplate, streamId);

                this._runningStreams.TryAdd(
                    streamId,
                    process);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, FFmpegManagerMessages.Error.FailedToStartStreamTemplate, ex.Message);

                return false;
            }
        }

        public bool StopStream(string streamId)
        {
            MyValidators.ValidateNotNullOrEmpty(streamId);

            if (!this._runningStreams.TryGetValue(streamId, out Process? process))
            {
               throw new ArgumentException($"Stream '{streamId}' is not running.");
            }

            _logger.LogInformation(FFmpegManagerMessages.Success.StreamStoppedTemplate, streamId);

            process.Kill();

            this._runningStreams.TryRemove(streamId, out _);

            process.Dispose();

            return true;
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
        public void KillAllRunningStreams()
        {
            foreach (string streamId in this._runningStreams.Keys.ToList())
            {
                this.StopStream(streamId);
            }
        }
    }
}